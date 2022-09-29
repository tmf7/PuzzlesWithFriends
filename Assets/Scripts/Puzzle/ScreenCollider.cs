using System.Collections.Generic;
using UnityEngine;

namespace JPWF
{
    /// <summary> Dynamically fits the edge colliders to the play space. </summary>
    [RequireComponent(typeof(EdgeCollider2D))]
    public class ScreenCollider : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;

        private EdgeCollider2D _screenCollider;
        private ScreenOrientation screenOrientation;
        private Resolution displayResolution;

        private void Awake()
        {
            _screenCollider = GetComponent<EdgeCollider2D>();
            ConfigureScreenCollider();
        }

        private void Update()
        {
            if (Screen.orientation != screenOrientation ||
                Screen.currentResolution.width != displayResolution.width ||
                Screen.currentResolution.height != displayResolution.height)
            {
                ConfigureScreenCollider();
            }
        }

        public void ConfigureScreenCollider()
        {
            var edges = new List<Vector2>();
            edges.Add(_mainCamera.ScreenToWorldPoint(Vector2.zero));
            edges.Add(_mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0.0f)));
            edges.Add(_mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)));
            edges.Add(_mainCamera.ScreenToWorldPoint(new Vector2(0.0f, Screen.height)));
            edges.Add(_mainCamera.ScreenToWorldPoint(Vector2.zero));
            _screenCollider.SetPoints(edges);
        }
    }
}