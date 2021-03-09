// GENERATED AUTOMATICALLY FROM 'Assets/Develop/GamePlay/GameLobby/DefaultModule/Script/DefaultModuleCtrl.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace GamePlay.GameLobby
{
    public class @DefaultModuleCtrl : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @DefaultModuleCtrl()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""DefaultModuleCtrl"",
    ""maps"": [
        {
            ""name"": ""Move"",
            ""id"": ""d1514ae3-9eb4-4f03-84dc-75dd88265e3b"",
            ""actions"": [
                {
                    ""name"": ""PC"",
                    ""type"": ""Value"",
                    ""id"": ""7177e46b-626c-4121-82af-17fee375c625"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1fc0e9e0-81fe-4b1b-9e1d-c80c8d1c51e0"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PC"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""de173932-6ca8-4bfc-ac0a-5583bb9d3dc9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PC"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f7c56127-6161-4359-ab22-278dabe03de9"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PC"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""56a0f056-9fa6-488e-86d1-71f1eba843a1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PC"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e5ad7b5f-fa73-4129-aeb6-367a30ffc91a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PC"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3f741906-e4a2-414a-94b6-42f054302d0a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PC"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Move
            m_Move = asset.FindActionMap("Move", throwIfNotFound: true);
            m_Move_PC = m_Move.FindAction("PC", throwIfNotFound: true);
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

        // Move
        private readonly InputActionMap m_Move;
        private IMoveActions m_MoveActionsCallbackInterface;
        private readonly InputAction m_Move_PC;
        public struct MoveActions
        {
            private @DefaultModuleCtrl m_Wrapper;
            public MoveActions(@DefaultModuleCtrl wrapper) { m_Wrapper = wrapper; }
            public InputAction @PC => m_Wrapper.m_Move_PC;
            public InputActionMap Get() { return m_Wrapper.m_Move; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MoveActions set) { return set.Get(); }
            public void SetCallbacks(IMoveActions instance)
            {
                if (m_Wrapper.m_MoveActionsCallbackInterface != null)
                {
                    @PC.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnPC;
                    @PC.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnPC;
                    @PC.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnPC;
                }
                m_Wrapper.m_MoveActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @PC.started += instance.OnPC;
                    @PC.performed += instance.OnPC;
                    @PC.canceled += instance.OnPC;
                }
            }
        }
        public MoveActions @Move => new MoveActions(this);
        public interface IMoveActions
        {
            void OnPC(InputAction.CallbackContext context);
        }
    }
}
