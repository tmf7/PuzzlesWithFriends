using UnityEngine;
using UnityEngine.InputSystem;
using static JPWF.PuzzleInputActionMap;

namespace JPWF
{
    public class Player : MonoBehaviour, IPlayerActions
    {
        [SerializeField] private Camera _mainCamera;

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

        public void OnDisable()
        {
            _inputActionMap.Player.Disable();
        }

        // TODO: detect when a Piece is clicked/dragged, and grab either the piece itself or its group to move
        // perform the move here, not in the piece, or the group


        public void OnSubmit(InputAction.CallbackContext context) { Debug.Log("OnSubmit"); }
        public void OnCancel(InputAction.CallbackContext context) { Debug.Log("OnCancel"); }
        public void OnPoint(InputAction.CallbackContext context) { Debug.Log("OnPoint"); }
        public void OnClick(InputAction.CallbackContext context) { Debug.Log("OnClick"); }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { Debug.Log("OnTrackedDeviceOrientation"); }

    }
}
