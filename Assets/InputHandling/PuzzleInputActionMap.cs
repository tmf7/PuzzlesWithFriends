// GENERATED AUTOMATICALLY FROM 'Assets/InputHandling/PuzzleInputActionMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace JPWF
{
    public class @PuzzleInputActionMap : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PuzzleInputActionMap()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PuzzleInputActionMap"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""6df0bf1d-8ddc-4254-b9c1-ecff6b8a7cd9"",
            ""actions"": [
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""50b7878c-187b-4a34-9a7b-5484e6f43b68"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""be9abd76-8918-464e-a46b-d4439da764f3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fce1d858-1365-4d5e-b498-ac1cd0217f82"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""acd147b0-68af-4b5c-9809-da41b00d3b9f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDeviceOrientation"",
                    ""type"": ""PassThrough"",
                    ""id"": ""3978720b-3eaa-4bd4-a837-ecd36e725d14"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""aee84b86-47d7-4f59-a2ff-1628937e77ea"",
                    ""path"": ""*/{Submit}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb7c29cd-5f19-497c-95c9-20b1ce8f1ce0"",
                    ""path"": ""*/{Cancel}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d0f2fbe-d192-4073-9fd6-c05eb8c4322e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e085a7e4-22f2-462c-b7ac-6d512077c722"",
                    ""path"": ""<Pen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4617cc3e-398b-4f17-9e5a-07be3b97a105"",
                    ""path"": ""<Touchscreen>/touch*/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4847136a-6cce-44b9-8ad2-392eaa069c5b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e352fa1-7274-44c6-9a57-8964f7f72565"",
                    ""path"": ""<Pen>/tip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1990983-b4da-4eb3-849b-bd8f666dc952"",
                    ""path"": ""<Touchscreen>/touch*/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0635100f-ee82-4e7f-a73c-0c29f1a989b8"",
                    ""path"": ""<GravitySensor>/gravity"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PointerControls"",
                    ""action"": ""TrackedDeviceOrientation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PointerControls"",
            ""bindingGroup"": ""PointerControls"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<GravitySensor>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Gyroscope>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Submit = m_Player.FindAction("Submit", throwIfNotFound: true);
            m_Player_Cancel = m_Player.FindAction("Cancel", throwIfNotFound: true);
            m_Player_Point = m_Player.FindAction("Point", throwIfNotFound: true);
            m_Player_Click = m_Player.FindAction("Click", throwIfNotFound: true);
            m_Player_TrackedDeviceOrientation = m_Player.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Submit;
        private readonly InputAction m_Player_Cancel;
        private readonly InputAction m_Player_Point;
        private readonly InputAction m_Player_Click;
        private readonly InputAction m_Player_TrackedDeviceOrientation;
        public struct PlayerActions
        {
            private @PuzzleInputActionMap m_Wrapper;
            public PlayerActions(@PuzzleInputActionMap wrapper) { m_Wrapper = wrapper; }
            public InputAction @Submit => m_Wrapper.m_Player_Submit;
            public InputAction @Cancel => m_Wrapper.m_Player_Cancel;
            public InputAction @Point => m_Wrapper.m_Player_Point;
            public InputAction @Click => m_Wrapper.m_Player_Click;
            public InputAction @TrackedDeviceOrientation => m_Wrapper.m_Player_TrackedDeviceOrientation;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @Submit.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                    @Submit.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                    @Submit.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                    @Cancel.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                    @Cancel.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                    @Cancel.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                    @Point.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoint;
                    @Point.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoint;
                    @Point.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoint;
                    @Click.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnClick;
                    @Click.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnClick;
                    @Click.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnClick;
                    @TrackedDeviceOrientation.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTrackedDeviceOrientation;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Submit.started += instance.OnSubmit;
                    @Submit.performed += instance.OnSubmit;
                    @Submit.canceled += instance.OnSubmit;
                    @Cancel.started += instance.OnCancel;
                    @Cancel.performed += instance.OnCancel;
                    @Cancel.canceled += instance.OnCancel;
                    @Point.started += instance.OnPoint;
                    @Point.performed += instance.OnPoint;
                    @Point.canceled += instance.OnPoint;
                    @Click.started += instance.OnClick;
                    @Click.performed += instance.OnClick;
                    @Click.canceled += instance.OnClick;
                    @TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);
        private int m_PointerControlsSchemeIndex = -1;
        public InputControlScheme PointerControlsScheme
        {
            get
            {
                if (m_PointerControlsSchemeIndex == -1) m_PointerControlsSchemeIndex = asset.FindControlSchemeIndex("PointerControls");
                return asset.controlSchemes[m_PointerControlsSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnSubmit(InputAction.CallbackContext context);
            void OnCancel(InputAction.CallbackContext context);
            void OnPoint(InputAction.CallbackContext context);
            void OnClick(InputAction.CallbackContext context);
            void OnTrackedDeviceOrientation(InputAction.CallbackContext context);
        }
    }
}
