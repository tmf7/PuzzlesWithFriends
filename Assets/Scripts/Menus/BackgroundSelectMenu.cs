using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace JPWF
{
    [RequireComponent(typeof(Animator))]
    public class BackgroundSelectMenu : MonoBehaviour
    {
        [SerializeField] private Image m_playAreaBackground;
        [SerializeField] private Image m_pieceSelectionBackground;
        [SerializeField] private RectTransform m_optionsParent;
        [SerializeField] private BackgroundImageButton m_backgroundButtonPrefab;
        [SerializeField] private BackgroundImageOptionData[] m_backgroundImageOptions;

        private Animator _backgroundOptionsAnimator;

        private readonly int _toggleVisibleParameter = Animator.StringToHash("ToggleVisible");

        private void Awake()
        {
            _backgroundOptionsAnimator = GetComponent<Animator>();

            for (int i = 0; i < m_backgroundImageOptions.Length; ++i)
            {
                BackgroundImageButton backgroundButton = Instantiate(m_backgroundButtonPrefab, m_optionsParent);
                backgroundButton.Init(this, m_backgroundImageOptions[i]);
            }

            // TODO: read the loaded user preferences and set the background, otherwise load a random background
            BackgroundImageOptionData backgroundData = m_backgroundImageOptions[Random.Range(0, m_backgroundImageOptions.Length)];
            ChangeBackgroundImage(backgroundData);
        }

        public void ChangeBackgroundImage(BackgroundImageOptionData backgroundData)
        {
            m_playAreaBackground.sprite = backgroundData.Background;
            m_pieceSelectionBackground.color = backgroundData.PrimaryColor;
        }

        public void ToggleVisible()
        {
            _backgroundOptionsAnimator.SetTrigger(_toggleVisibleParameter);
        }
    }
}