using UnityEngine;
using UnityEngine.EventSystems;

namespace JPWF
{
    [RequireComponent(typeof(Animator))]
    public class PushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Animator _animator;
        private readonly int _pressedTriggerParameter = Animator.StringToHash("Pressed");
        private readonly int _releasedTriggerParameter = Animator.StringToHash("Released");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _animator.SetTrigger(_pressedTriggerParameter);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AudioManager.PlayButtonPressSfx();
            _animator.SetTrigger(_releasedTriggerParameter);
        }
    }
}