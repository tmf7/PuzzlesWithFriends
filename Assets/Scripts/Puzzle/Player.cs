using UnityEngine;
using UnityEngine.InputSystem;
using static JPWF.PuzzleInputActionMap;

namespace JPWF
{
    //[RequireComponent(typeof(Camera))]
    public class Player : MonoBehaviour, IPlayerActions
    {
        private Camera _mainCamera;

        private PuzzleInputActionMap _inputActionMap;

        public void OnEnable()
        {
            if (_inputActionMap == null)
            {
                _inputActionMap = new PuzzleInputActionMap();

                // Tell the "gameplay" action map that we want to get told about
                // when actions get triggered.
                _inputActionMap.Player.SetCallbacks(this);
            }
            _inputActionMap.Player.Enable();
        }

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
        }

        public void OnDisable()
        {
            _inputActionMap.Player.Disable();
        }

        // TODO: detect when a Piece is clicked/dragged, and grab either the piece itself or its group to move
        // perform the move here, not in the piece, or the group

        public void OnSubmit(InputAction.CallbackContext context) {/* if (context.performed)*/ Debug.Log("OnSubmit");}
        public void OnCancel(InputAction.CallbackContext context) {/* if (context.performed)*/ Debug.Log("OnCancel"); } // TODO: this fires when the back button/escape is pressed
        public void OnPress(InputAction.CallbackContext context) { /*if (context.performed)*/ Debug.Log("OnPress"); }
        public void OnPoint(InputAction.CallbackContext context) { /*if (context.performed)*/ Debug.Log("OnPoint"); } // this fires whenever the cursor moves on the screen (drag or not)
        public void OnClick(InputAction.CallbackContext context) { /*if (context.performed)*/ Debug.Log("OnClick"); }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { /*if (context.performed)*/ Debug.Log("OnTrackedDeviceOrientation"); }

    }
}
