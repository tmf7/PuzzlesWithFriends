using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JPWF
{
    // TODO: these are what need to be downloaded from the database
    // in addition to the player's current indexed puzzle piece positions for this puzzle.
    // SOLUTION: downloading two textures then generating 1000 piece textures is faster than:
    // uploading an image, splitting on the server, then downloading 1000 piece textures
    [Serializable]
    public struct PuzzleTemplate
    {
        public Texture colorKeyMap;
        public Texture colorKeyEdges;
        public Vector3Int[] colorKeyValues; 
    }

    public class PuzzlePieceTextureGenerator : MonoBehaviour
    {
        [SerializeField] private Shader _puzzlePieceCutterShader;
        [SerializeField] private ComputeShader _boundingBoxComputeShader;
        [SerializeField] private PuzzleTemplate _puzzleTemplate;
        [SerializeField] private Texture _puzzleImage;
        [SerializeField] private PuzzlePiece _puzzlePiecePrefab;

        private Material _puzzlePieceCutterMaterial;

        private static readonly int pieceColorKeyID = Shader.PropertyToID("_PieceColorKey");

        private const int RENDERTEXTURE_DIM = 1024;
        private const int NUM_THREAD_GROUPS = 32;
        private const float COLOR_RANGE = 255.0f;
        private const float PIXELS_PER_UNIT = 1.0f / 256.0f; // emperical based on the test texture being 1024*1024
        private const float WORLDSPACE_Z = 0.0f; // the camera is orthographic, so this only needs to be closer than the background offset

        private List<PuzzlePiece> _puzzle;

        private void Start()
        {
            _puzzlePieceCutterMaterial = new Material(_puzzlePieceCutterShader);

            _puzzle = GeneratePuzzlePieces(_puzzleTemplate, _puzzleImage);
            Debug.Log($"GENERATED: {_puzzle.Count} puzzle pieces");
        }

        // TODO: call this from the game manager when a level, or puzzle editor, loads
        public List<PuzzlePiece> GeneratePuzzlePieces(PuzzleTemplate puzzleTemplate, Texture puzzleImage)
        {
            _puzzlePieceCutterMaterial.SetTexture("_ColorKeyMap", puzzleTemplate.colorKeyMap);
            _puzzlePieceCutterMaterial.SetTexture("_ColorKeyEdges", puzzleTemplate.colorKeyEdges);
            _puzzlePieceCutterMaterial.SetTexture("_PuzzleImage", puzzleImage);

            var puzzlePieces = new List<PuzzlePiece>();
            var tempRenderTexture = RenderTexture.GetTemporary(RENDERTEXTURE_DIM, RENDERTEXTURE_DIM, 0, RenderTextureFormat.ARGB32);
            var originalActiveRenderTexture = RenderTexture.active;
            int pieceCount = 0;

            foreach (var colorKeyValue in puzzleTemplate.colorKeyValues)
            {
                SetColorKey(colorKeyValue);

                RenderTexture.active = tempRenderTexture;
                GL.Clear(true, true, Color.clear);
                Graphics.Blit(null, tempRenderTexture, _puzzlePieceCutterMaterial);

                uint[] minMaxBounds = FindBounds(tempRenderTexture);

                uint width = minMaxBounds[2] - minMaxBounds[0]; 
                uint height = minMaxBounds[3] - minMaxBounds[1];

                if (width > 0 && height > 0)
                {
                    pieceCount++;
                    int potWidth = (int)NextPowerOfTwo(width);
                    int potHeight = (int)NextPowerOfTwo(height);
                    
                    RenderTexture puzzlePieceTexture = CreateTexture(tempRenderTexture, minMaxBounds, potWidth, potHeight);

                    var worldCenterOffset = new Vector3((minMaxBounds[2] + minMaxBounds[0]) * 0.5f,
                                                        (minMaxBounds[3] + minMaxBounds[1]) * 0.5f,
                                                        WORLDSPACE_Z) * PIXELS_PER_UNIT;
                    var textureCopyOffset = new Vector3(0.5f * (potWidth - width), 
                                                        0.5f * (potHeight - height), 
                                                        0.0f) * PIXELS_PER_UNIT;

                    // FIXME: this positions the piece relative to this generator, rather than the play space
                    var debugStartPosition = transform.position + worldCenterOffset;

                    // TODO: worldPosition should be set externally, using the solvedWorldPosition is just a debug test
                    var newPuzzlePiece = Instantiate(_puzzlePiecePrefab, debugStartPosition, Quaternion.identity, transform);
                    newPuzzlePiece.name = $"{_puzzlePiecePrefab.name} ({pieceCount})"; 

                    var size = new Vector3(width, height, 0.0f) * PIXELS_PER_UNIT;
                    var colliderBounds = new Bounds(textureCopyOffset, size);

                    newPuzzlePiece.Init(puzzlePieceTexture, colliderBounds);
                    puzzlePieces.Add(newPuzzlePiece);
                }
            }

            foreach (PuzzlePiece puzzlePiece in puzzlePieces)
            {
                puzzlePiece.InitSolutionNeighborhood();
            }

            RenderTexture.active = originalActiveRenderTexture;
            RenderTexture.ReleaseTemporary(tempRenderTexture);

            return puzzlePieces;
        }

        /// <summary> Sets what colorKey will be used when blitting the puzzleImage using the colorKeyMap. </summary>
        private void SetColorKey(Vector3Int rgb)
        {
            // TODO: this clamp is unnecessary if the key JSON is setup correctly
            rgb.Clamp(Vector3Int.zero, Vector3Int.one * (int)COLOR_RANGE);
            var color = (Vector3)rgb / COLOR_RANGE;

            // color keys are not currently encoded using alpha because
            // alpha is used to find the bounding box of the puzzle piece
            var colorKey = new Color(color.x, color.y, color.z, 1.0f);

            _puzzlePieceCutterMaterial.SetColor(pieceColorKeyID, colorKey);
        }

        /// <summary> Finds the min-max AABB of all opaque pixels on the shaderOutputTexture. </summary>
        private uint[] FindBounds(RenderTexture shaderOutputTexture)
        {
            // x-min, y-min, x-max, y-max
            var boundsBuffer = new ComputeBuffer(1, sizeof(uint) * 4);

            int kernelIndex = _boundingBoxComputeShader.FindKernel("InitBoundingBoxBuffer");
            _boundingBoxComputeShader.SetBuffer(kernelIndex, "boundsBuffer", boundsBuffer);
            _boundingBoxComputeShader.Dispatch(kernelIndex, 1, 1, 1);

            kernelIndex = _boundingBoxComputeShader.FindKernel("FindBoundingBox");
            _boundingBoxComputeShader.SetTexture(kernelIndex, "shaderOutputTexture", shaderOutputTexture);
            _boundingBoxComputeShader.SetBuffer(kernelIndex, "boundsBuffer", boundsBuffer);
            _boundingBoxComputeShader.Dispatch(kernelIndex, shaderOutputTexture.width / NUM_THREAD_GROUPS, shaderOutputTexture.height / NUM_THREAD_GROUPS, 1);

            var minMaxBounds = new uint[4];
            boundsBuffer.GetData(minMaxBounds);
            boundsBuffer.Release();
            boundsBuffer = null;

            return minMaxBounds;
        }

        /// <summary> Copies the shaderOutputTexture onto a new rendertexture of the given size </summary>
        private RenderTexture CreateTexture(RenderTexture shaderOutputTexture, uint[] minMaxBounds, int newWidth, int newHeight)
        {
            var boundsBuffer = new ComputeBuffer(1, sizeof(uint) * 4);
            boundsBuffer.SetData(minMaxBounds);

            var puzzlePieceTexture = new RenderTexture(newWidth, newHeight, 0, RenderTextureFormat.ARGB32);
            puzzlePieceTexture.enableRandomWrite = true;
            puzzlePieceTexture.Create();

            int kernelIndex = _boundingBoxComputeShader.FindKernel("CopyTexture");
            _boundingBoxComputeShader.SetBuffer(kernelIndex, "boundsBuffer", boundsBuffer);
            _boundingBoxComputeShader.SetTexture(kernelIndex, "shaderOutputTexture", shaderOutputTexture);
            _boundingBoxComputeShader.SetTexture(kernelIndex, "puzzlePieceTexture", puzzlePieceTexture);
            _boundingBoxComputeShader.Dispatch(kernelIndex, shaderOutputTexture.width / NUM_THREAD_GROUPS, shaderOutputTexture.height / NUM_THREAD_GROUPS, 1);

            boundsBuffer.Dispose();
            boundsBuffer = null;

            return puzzlePieceTexture;
        }

        // 32 bit integer log2 twiddle
        private uint NextPowerOfTwo(uint value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;

            return value;
        }
    }
}