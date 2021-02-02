// GENERATED AUTOMATICALLY FROM 'Assets/Controler/Player_Gameplay_Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Player_Gameplay_Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Player_Gameplay_Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player_Gameplay_Controls"",
    ""maps"": [
        {
            ""name"": ""Ground"",
            ""id"": ""313db472-7d29-4d98-996f-59fd05101504"",
            ""actions"": [
                {
                    ""name"": ""move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""321dd71a-66b0-4ca6-a254-86f0c2f0f62e"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""jump"",
                    ""type"": ""Button"",
                    ""id"": ""b1f08a49-de6a-476e-bf0e-e1fab86ebc3d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""YoyoAction"",
                    ""type"": ""Button"",
                    ""id"": ""48b78a4a-069b-4642-b07a-d6c8af78331c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Hand"",
                    ""type"": ""Value"",
                    ""id"": ""7c7943c2-7fd0-406d-bbfe-537a7a7e0048"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""9d72bf2c-5b59-448e-ba1f-914e95d6dfb3"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""1f850089-da89-4a43-a4b3-ad53662be7ec"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=10)"",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""5c7eb5ef-f9d7-4ad8-a387-ca72f4891029"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=10)"",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""b0fdb72c-c0b6-41d8-8a7e-a8453c2fe002"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.2)"",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""0485719f-71a2-4d72-adf2-b1efcf8e0121"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""cb9cc539-9e4c-4126-97ea-792a7febc41d"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6e04e28f-b5ca-4629-883b-21184c19b2b7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93d5bf9c-6fcc-4aca-879c-c6eca5b2453b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c3f4cc4c-499b-4f1f-a91c-ada5520c4bb6"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.3)"",
                    ""groups"": """",
                    ""action"": ""Hand"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc9eb128-a0f5-4de0-a6aa-7a7dc21764f1"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""YoyoAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Ground
        m_Ground = asset.FindActionMap("Ground", throwIfNotFound: true);
        m_Ground_move = m_Ground.FindAction("move", throwIfNotFound: true);
        m_Ground_jump = m_Ground.FindAction("jump", throwIfNotFound: true);
        m_Ground_YoyoAction = m_Ground.FindAction("YoyoAction", throwIfNotFound: true);
        m_Ground_Hand = m_Ground.FindAction("Hand", throwIfNotFound: true);
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

    // Ground
    private readonly InputActionMap m_Ground;
    private IGroundActions m_GroundActionsCallbackInterface;
    private readonly InputAction m_Ground_move;
    private readonly InputAction m_Ground_jump;
    private readonly InputAction m_Ground_YoyoAction;
    private readonly InputAction m_Ground_Hand;
    public struct GroundActions
    {
        private @Player_Gameplay_Controls m_Wrapper;
        public GroundActions(@Player_Gameplay_Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @move => m_Wrapper.m_Ground_move;
        public InputAction @jump => m_Wrapper.m_Ground_jump;
        public InputAction @YoyoAction => m_Wrapper.m_Ground_YoyoAction;
        public InputAction @Hand => m_Wrapper.m_Ground_Hand;
        public InputActionMap Get() { return m_Wrapper.m_Ground; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GroundActions set) { return set.Get(); }
        public void SetCallbacks(IGroundActions instance)
        {
            if (m_Wrapper.m_GroundActionsCallbackInterface != null)
            {
                @move.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnMove;
                @move.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnMove;
                @move.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnMove;
                @jump.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnJump;
                @jump.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnJump;
                @jump.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnJump;
                @YoyoAction.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnYoyoAction;
                @YoyoAction.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnYoyoAction;
                @YoyoAction.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnYoyoAction;
                @Hand.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnHand;
                @Hand.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnHand;
                @Hand.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnHand;
            }
            m_Wrapper.m_GroundActionsCallbackInterface = instance;
            if (instance != null)
            {
                @move.started += instance.OnMove;
                @move.performed += instance.OnMove;
                @move.canceled += instance.OnMove;
                @jump.started += instance.OnJump;
                @jump.performed += instance.OnJump;
                @jump.canceled += instance.OnJump;
                @YoyoAction.started += instance.OnYoyoAction;
                @YoyoAction.performed += instance.OnYoyoAction;
                @YoyoAction.canceled += instance.OnYoyoAction;
                @Hand.started += instance.OnHand;
                @Hand.performed += instance.OnHand;
                @Hand.canceled += instance.OnHand;
            }
        }
    }
    public GroundActions @Ground => new GroundActions(this);
    public interface IGroundActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnYoyoAction(InputAction.CallbackContext context);
        void OnHand(InputAction.CallbackContext context);
    }
}
