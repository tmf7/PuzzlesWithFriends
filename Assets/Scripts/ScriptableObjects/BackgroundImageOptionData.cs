using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JPWF
{
    /// <summary>
    /// Describes a background texture used while solving a puzzle, as well as its corresponding
    /// puzzle piece tray background color.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBackgroundImageOption", menuName = "Data/Background Image Option")]
    public class BackgroundImageOptionData : ScriptableObject
    {
        [SerializeField] private Sprite m_background;
        [SerializeField] private Color m_primaryColor;

        public Sprite Background => m_background;
        public Color PrimaryColor => m_primaryColor;

        #region EDITOR_CODE
#if UNITY_EDITOR
        [CustomEditor(typeof(BackgroundImageOptionData))]
        private class BackgroundImageOptionDataEditor : Editor
        {
            private BackgroundImageOptionData m_backgroundOption;

            private void Awake()
            {
                m_backgroundOption = (BackgroundImageOptionData)target;
            }

            public override void OnInspectorGUI()
            {
                var initialBackground = m_backgroundOption.Background;
                base.OnInspectorGUI();

                // auto-update the primary color based on an average color at the center of the sprite
                if (initialBackground != m_backgroundOption.Background && m_backgroundOption.Background != null)
                {
                    m_backgroundOption.m_primaryColor = Color.Lerp(m_backgroundOption.Background.texture.GetPixelBilinear(0.5f, 0.5f),
                           m_backgroundOption.Background.texture.GetPixelBilinear(0.6f, 0.5f),
                           0.5f);
                }
            }
        }
#endif
#endregion EDITOR_CODE
    }
}