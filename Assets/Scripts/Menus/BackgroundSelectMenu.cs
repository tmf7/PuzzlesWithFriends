using UnityEngine;
using UnityEngine.UI;

namespace JPWF
{
    [RequireComponent(typeof(Animator))]
    public class BackgroundSelectMenu : MonoBehaviour
    {
        [SerializeField] private Image _playAreaBackground;
        [SerializeField] private Image _pieceSelectionBackground;

        private Animator _backgroundOptionsAnimator;

        private readonly int _toggleVisibleParameter = Animator.StringToHash("ToggleVisible");

        private void Awake()
        {
            _backgroundOptionsAnimator = GetComponent<Animator>();
        }

        public void ChangeBackgroundImage(Sprite background, Color primaryColor)
        {
            _playAreaBackground.sprite = background;
            _pieceSelectionBackground.color = primaryColor;
        }

        public void ToggleVisible()
        {
            _backgroundOptionsAnimator.SetTrigger(_toggleVisibleParameter);
        }
    }
}