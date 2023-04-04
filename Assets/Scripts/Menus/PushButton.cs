using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JPWF
{
    [RequireComponent(typeof(Animator))]
    public class PushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image _targetImage;
        [SerializeField] private Sprite[] _sprites;

        private Animator _animator;
        private int _currentSprite = 0;

        private readonly int _pressedTriggerParameter = Animator.StringToHash("Pressed");
        private readonly int _releasedTriggerParameter = Animator.StringToHash("Released");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _targetImage.sprite = _sprites[_currentSprite];
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _animator.SetTrigger(_pressedTriggerParameter);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AudioManager.PlayButtonPressSfx();
            _animator.SetTrigger(_releasedTriggerParameter);
            _currentSprite = ++_currentSprite % _sprites.Length;
            _targetImage.sprite = _sprites[_currentSprite];
        }
    }
}