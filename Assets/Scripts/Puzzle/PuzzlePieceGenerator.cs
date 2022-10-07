using System.Collections.Generic;
using UnityEngine;

namespace JPWF
{
    public class PuzzlePieceGenerator : MonoBehaviour
    {
        [SerializeField] private Shader _puzzlePieceCutterShader;
        [SerializeField] private ComputeShader _boundingBoxComputeShader;
        [SerializeField] private PuzzlePiece _puzzlePiecePrefab;
        [SerializeField] private Camera _mainCamera;

        [Header("DEBUG TEST DATA")]
        [SerializeField] private Texture _puzzleImage;
        [SerializeField] private PuzzleTemplateData _puzzleTemplate;

        private const int NUM_THREAD_GROUPS = 32;
        private const float WORLDSPACE_Z = 0.0f; // the camera is orthographic, so this only needs to be closer than the background offset

        private void Start()
        {
            var debugPuzzle = GeneratePuzzlePieces(_puzzleTemplate, _puzzleImage);
            Debug.Log($"GENERATED: {debugPuzzle.Count} puzzle pieces");
        }

        private Color GetColorKey(Vector3Int rgb)
        {
            rgb.Clamp(Vector3Int.zero, Vector3Int.one * byte.MaxValue);
            var color = (Vector3)rgb / byte.MaxValue;

            // color keys are not currently encoded using alpha channel
            return new Color(color.x, color.y, color.z, 1.0f);
        }

        // TODO: call this from the game manager when a puzzle loads
        public List<PuzzlePiece> GeneratePuzzlePieces(PuzzleTemplateData puzzleTemplate, Texture puzzleImage)
        {
           var puzzlePieces = new List<PuzzlePiece>();
            var puzzlePieceData = new List<(Color colorKey, uint[] minMaxBounds)>();
            float maxTilingFactor = 0.0f;

            // find the maxTilingFactor so all pieces of this template fit in their final quad's UVs
            foreach (Vector3Int colorKeyValue in puzzleTemplate.ColorKeyValues)
            {
                var colorKey = GetColorKey(colorKeyValue);
                uint[] minMaxBounds = FindBounds(puzzleTemplate.ColorKeyMap, colorKey);
                uint width = minMaxBounds[2] - minMaxBounds[0]; 
                uint height = minMaxBounds[3] - minMaxBounds[1];

                if (width > 0 && height > 0)
                {
                    puzzlePieceData.Add((colorKey, minMaxBounds));
                    float uvWidth = width / (float)puzzleTemplate.ColorKeyMap.width;   // scale based on piece width 
                    float uvHeight = height / (float)puzzleTemplate.ColorKeyMap.height; // scale based on piece height

                    // the larger the tilingFactor, the smaller the final image
                    maxTilingFactor = Mathf.Max(uvWidth, uvHeight, maxTilingFactor);
                }
            }

            // generate pieces with unique material scale and offsets
            for (int i = 0; i < puzzlePieceData.Count; ++i)
            {
                uint[] minMaxBounds = puzzlePieceData[i].minMaxBounds;
                float uvWidth = (minMaxBounds[2] - minMaxBounds[0]) / (float)puzzleTemplate.ColorKeyMap.width;
                float uvHeight = (minMaxBounds[3] - minMaxBounds[1]) / (float)puzzleTemplate.ColorKeyMap.height;

                // Fit each piece to their own quad, centered on the quad's pivot point:
                // width and height are converted from pixels to ColorKeyMap UV coordinates
                float uMin = minMaxBounds[0] / (float)puzzleTemplate.ColorKeyMap.width;
                float vMin = minMaxBounds[1] / (float)puzzleTemplate.ColorKeyMap.height;

                // center the unscaled UVs, then shift the scaled UVs back along its uniform scaling vector
                var uniformTilingOffset = Vector2.one * 0.5f * (1.0f - maxTilingFactor);
                float xOffset = uMin - ((1.0f - uvWidth) * 0.5f) + uniformTilingOffset.x;
                float yOffset = vMin - ((1.0f - uvHeight) * 0.5f) + uniformTilingOffset.y;

                var puzzlePieceMaterial = new Material(_puzzlePieceCutterShader);
                puzzlePieceMaterial.SetTexture("_ColorKeyMap", puzzleTemplate.ColorKeyMap);
                puzzlePieceMaterial.SetTexture("_ColorKeyEdges", puzzleTemplate.ColorKeyEdges);
                puzzlePieceMaterial.SetTexture("_MainTex", puzzleImage);
                puzzlePieceMaterial.SetColor("_PieceColorKey", puzzlePieceData[i].colorKey);
                puzzlePieceMaterial.SetFloat("_GraphicTiling", maxTilingFactor);
                puzzlePieceMaterial.SetVector("_PivotOffset", new Vector4(xOffset, yOffset, 0.0f, 0.0f));

                float centerOffsetScale = 1.0f / (puzzleTemplate.ColorKeyMap.width * maxTilingFactor);
                var worldCenterOffset = new Vector3((minMaxBounds[2] + minMaxBounds[0]) * 0.5f,
                                                    (minMaxBounds[3] + minMaxBounds[1]) * 0.5f,
                                                    WORLDSPACE_Z) * centerOffsetScale;

                var solvedWorldPosition = transform.position + worldCenterOffset;

                // FIXME(maybe): ensure the solvedWorldPosition starts from a corner of the play space
                var newPuzzlePiece = Instantiate(_puzzlePiecePrefab, solvedWorldPosition, Quaternion.identity, transform);
                newPuzzlePiece.name = $"{_puzzlePiecePrefab.name} ({i + 1})";

                var size = new Vector3(uvWidth / maxTilingFactor, uvHeight / maxTilingFactor, 0.0f);
                var colliderBounds = new Bounds(Vector3.zero, size);

                newPuzzlePiece.Init(puzzlePieceMaterial, colliderBounds, _mainCamera);
                puzzlePieces.Add(newPuzzlePiece);
            }

            foreach (PuzzlePiece puzzlePiece in puzzlePieces)
            {
                puzzlePiece.InitSolutionNeighborhood();
            }

            return puzzlePieces;
        }

        /// <summary> Finds the min-max AABB (in pixels) of all opaque colorkeyed pixels on the colorKeyMap. </summary>
        private uint[] FindBounds(Texture colorKeyMap, Color colorKeyValue)
        {
            // x-min, y-min, x-max, y-max
            var boundsBuffer = new ComputeBuffer(1, sizeof(uint) * 4);

            int kernelIndex = _boundingBoxComputeShader.FindKernel("InitBoundingBoxBuffer");
            _boundingBoxComputeShader.SetBuffer(kernelIndex, "boundsBuffer", boundsBuffer);
            _boundingBoxComputeShader.Dispatch(kernelIndex, 1, 1, 1);

            kernelIndex = _boundingBoxComputeShader.FindKernel("FindBoundingBox");
            _boundingBoxComputeShader.SetTexture(kernelIndex, "colorKeyMap", colorKeyMap);
            _boundingBoxComputeShader.SetVector("colorKeyValue", new Vector4(colorKeyValue.r, colorKeyValue.g, colorKeyValue.b, colorKeyValue.a));
            _boundingBoxComputeShader.SetBuffer(kernelIndex, "boundsBuffer", boundsBuffer);

            // ensure all colorKeyMaps are divisible by NUM_THREAD_GROUPS (32) on each axis
            _boundingBoxComputeShader.Dispatch(kernelIndex, colorKeyMap.width / NUM_THREAD_GROUPS, colorKeyMap.height / NUM_THREAD_GROUPS, 1);

            var minMaxBounds = new uint[4];
            boundsBuffer.GetData(minMaxBounds);
            boundsBuffer.Release();
            boundsBuffer = null;

            return minMaxBounds;
        }
    }
}