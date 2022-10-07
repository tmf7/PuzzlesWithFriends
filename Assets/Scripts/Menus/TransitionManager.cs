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

        private readonly int _mainTexProperty = Shader.PropertyToID("_MainTex");
        private readonly int _cutoffProperty = Shader.PropertyToID("_Cutoff");

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
                _transitionMaterial.SetFloat(_cutoffProperty, MIN_CUTOFF);
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
                _instance._transitionMaterial.SetTexture(_instance._mainTexProperty, transitionTexture);
                _instance._transitionImage.SetMaterialDirty();
            }

            if (_instance._transitionCoroutine != null)
            { 
                _instance.StopCoroutine(_instance._transitionCoroutine);
            }

            _instance._transitionCoroutine = _instance.StartCoroutine(_instance.TransitionCoroutine(to, complete));

        }

        private IEnumerator TransitionCoroutine(bool to, Action complete)
        {
            float cutoff = _transitionMaterial.GetFloat(_cutoffProperty);
            float target = to ? MAX_CUTOFF : MIN_CUTOFF;

            _transitionCanvas.enabled = true;

            while (Mathf.Abs(_transitionMaterial.GetFloat(_cutoffProperty) - target) > 0.0f)
            {
                cutoff = Mathf.MoveTowards(cutoff, target, _transitionSpeed * Time.deltaTime);
                _transitionMaterial.SetFloat(_cutoffProperty, cutoff);
                _instance._transitionImage.SetMaterialDirty();
                yield return null;
            }

            _transitionMaterial.SetFloat(_cutoffProperty, target);
            _instance._transitionImage.SetMaterialDirty();
            _transitionCanvas.enabled = to;
            _transitionCoroutine = null;
            complete?.Invoke();
        }
    }
}