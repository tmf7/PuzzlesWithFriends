using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JPWF
{
    public class InGameMenuButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private BackgroundSelectMenu _backgroundSelectMenu;
        [SerializeField] private RectTransform _menuButtonImageRectTransform;
        [SerializeField] private GameObject _menuButtonTextObject;

        private Coroutine _animationCoroutine;
        private bool _isOpen;

        private readonly Quaternion FLIP_ROTATION = Quaternion.AngleAxis(180.0f, Vector3.forward);
        private const float ANIMATION_TIME = 0.25f;
        private const float MIN_X = 0.303f;
        private const float MAX_X = 1.707f;
        private const float MIN_SCALE = 0.5f;

        private void Awake()
        {
            TransitionManager.Transition(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_animationCoroutine == null)
            {
                _isOpen = !_isOpen;
                _backgroundSelectMenu.ToggleVisible();
                _menuButtonTextObject.SetActive(!_isOpen);
                _animationCoroutine = StartCoroutine(Press());
            }
        }

        private IEnumerator Press()
        {
            float timeRemaining = ANIMATION_TIME;
            Quaternion initialRotation = _menuButtonImageRectTransform.rotation;
            Quaternion finalRotation = FLIP_ROTATION * initialRotation;

            while (timeRemaining > 0.0f)
            {
                timeRemaining -= Time.deltaTime;
                float t = timeRemaining / ANIMATION_TIME;
                float x = Mathf.Lerp(MIN_X, MAX_X, t);
                _menuButtonImageRectTransform.localScale = Vector3.one * (Mathf.Pow(x - 1, 2) + MIN_SCALE); // scale along a parabola
                _menuButtonImageRectTransform.rotation = Quaternion.Lerp(initialRotation, finalRotation, 1.0f - t);
                yield return null;
            }

            _menuButtonImageRectTransform.localScale = Vector3.one;
            _menuButtonImageRectTransform.rotation = finalRotation;
            _animationCoroutine = null;
        }
    }
}