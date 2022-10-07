using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace JPWF
{
    [CreateAssetMenu(fileName = "NewPuzzleTemplate", menuName = "Puzzle Template")]
    public class PuzzleTemplateData : ScriptableObject
    {
        [Tooltip("Ensure the texture is marked read-write, wrapmode clamp, point (no filter), override compression to RGBA 32bit")]
        [SerializeField] private Texture2D _colorKeyMap;
        [SerializeField] private Texture _colorKeyEdges;
        [SerializeField] private Vector3Int[] _colorKeyValues;

        /// <summary> Color coded "UV" map of each piece of this puzzle template. </summary>
        public Texture ColorKeyMap => _colorKeyMap;

        /// <summary> Edges corresponding to color mapped piece of this puzzle template. </summary>
        public Texture ColorKeyEdges => _colorKeyEdges;

        /// <summary> Colors that uniquely identify each puzzle piece in the <see cref="ColorKeyMap"/>. One color per piece. (RGB: 0-255) (A: 255) </summary>
        public Vector3Int[] ColorKeyValues => _colorKeyValues;

        #region EDITOR_CODE
#if UNITY_EDITOR
        private class ColorCompare : IComparer<Vector3Int>
        {
            public int Compare(Vector3Int lhs, Vector3Int rhs)
            {
                if (lhs.x > rhs.x)
                    return 1;
                if (lhs.x < rhs.x)
                    return -1;
                if (lhs.y > rhs.y)
                    return 1;
                if (lhs.y < rhs.y)
                    return -1;
                if (lhs.z > rhs.z)
                    return 1;
                if (lhs.z < rhs.z)
                    return -1;

                return 0;
            }
        }

        /// <summary>
        /// Extracts the distinct RGB values 0-255 from the given <see cref="ColorKeyMap"/>. 
        /// Colors are stored as Vector3Int to maximize precision before submitting to the shader with alpha set to 255.
        /// </summary>
        private void UpdateColorKeys()
        {
            var colorKeyList = _colorKeyMap.GetPixels().Distinct()
                                                       .Where(color => color.a > 0)
                                                       .Select(color => new Vector3Int((int)(color.r * byte.MaxValue), (int)(color.g * byte.MaxValue), (int)(color.b * byte.MaxValue)))
                                                       .ToList();
            colorKeyList.Sort(new ColorCompare());
            _colorKeyValues = colorKeyList.ToArray();
            Debug.Log($"Found {_colorKeyValues.Length} unique opaque color keys in {nameof(_colorKeyMap)} [{_colorKeyMap.name}]");
        }

        [CustomEditor(typeof(PuzzleTemplateData))]
        private class PuzzleTemplateDataEditor : Editor
        {
            private PuzzleTemplateData _puzzleTemplate;

            private void Awake()
            {
                _puzzleTemplate = (PuzzleTemplateData)target;
            }

            public override void OnInspectorGUI()
            {
                if (GUILayout.Button(nameof(PuzzleTemplateData.UpdateColorKeys)))
                {
                    _puzzleTemplate.UpdateColorKeys();
                }

                base.OnInspectorGUI();
            }
        }
#endif
        #endregion EDITOR_CODE
    }
}