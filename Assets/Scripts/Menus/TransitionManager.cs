using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JPWF
{
    [RequireComponent(typeof(Canvas))]
    public class TransitionManager : MonoBehaviour
    {
        private static TransitionManager _instance;

        [SerializeField] private Image _transitionImage;
        [SerializeField] [Min(0.1f)] private float _transitionSpeed = 1.0f;

        private Canvas _transitionCanvas;
        private Material _transitionMaterial;
        private Coroutine _transitionCoroutine;

        private int MainTexProperty = Shader.PropertyToID("_MainTex");
        private int CutoffProperty = Shader.PropertyToID("_Cutoff");

        private const float MIN_CUTOFF = 0.0f;
        private const float MAX_CUTOFF = 1.1f;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                _transitionCanvas = GetComponent<Canvas>();
                _transitionMaterial = _transitionImage.material;
                _transitionMaterial.SetFloat(CutoffProperty, MIN_CUTOFF);
                _transitionCanvas.enabled = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>Builds the transition image on its canvas to cover/uncover the screen.</summary>
        /// <param name="to">If true the transition texture will move from its current coverage to fully covered. 
        ///                  If false the transition texture will move from its current coverage to fully uncovered.</param>
        /// <param name="transitionTexture">If set, then the transition texture will change. If left null the transition texture will retain its current value. </param>
        /// <param name="complete">Callback when the current transition action is complete. </param>
        public static void Transition(bool to, Texture transitionTexture = null, Action complete = null)
        {
            if (transitionTexture != null)
            {
                _instance._transitionMaterial.SetTexture(_instance.MainTexProperty, transitionTexture);
            }

            if (_instance._transitionCoroutine != null)
            { 
                _instance.StopCoroutine(_instance._transitionCoroutine);
            }

            _instance._transitionCoroutine = _instance.StartCoroutine(_instance.TransitionCoroutine(to, complete));

        }

        private IEnumerator TransitionCoroutine(bool to, Action complete)
        {
            float cutoff = _transitionMaterial.GetFloat(CutoffProperty);
            float target = to ? MAX_CUTOFF : MIN_CUTOFF;

            _transitionCanvas.enabled = true;

            while (Mathf.Abs(_transitionMaterial.GetFloat(CutoffProperty) - target) > 0.0f)
            {
                cutoff = Mathf.MoveTowards(cutoff, target, _transitionSpeed * Time.deltaTime);
                _transitionMaterial.SetFloat(CutoffProperty, cutoff);
                yield return null;
            }

            _transitionMaterial.SetFloat(CutoffProperty, target);
            _transitionCanvas.enabled = to;
            _transitionCoroutine = null;
            complete?.Invoke();
        }
    }
}