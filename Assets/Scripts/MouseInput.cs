// GENERATED AUTOMATICALLY FROM 'Assets/Mouse.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MouseInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MouseInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Mouse"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""c8eb1a29-2d86-414c-a686-42cafc6a5b92"",
            ""actions"": [
                {
                    ""name"": ""Mouse position"",
                    ""type"": ""Value"",
                    ""id"": ""55bde72a-ad32-4e85-92ec-9b41cfcad71e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""334709b6-248c-413b-8bda-9e146740f12a"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_Mouseposition = m_Mouse.FindAction("Mouse position", throwIfNotFound: true);
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

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_Mouseposition;
    public struct MouseActions
    {
        private @MouseInput m_Wrapper;
        public MouseActions(@MouseInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Mouseposition => m_Wrapper.m_Mouse_Mouseposition;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @Mouseposition.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseposition;
                @Mouseposition.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseposition;
                @Mouseposition.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseposition;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Mouseposition.started += instance.OnMouseposition;
                @Mouseposition.performed += instance.OnMouseposition;
                @Mouseposition.canceled += instance.OnMouseposition;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);
    public interface IMouseActions
    {
        void OnMouseposition(InputAction.CallbackContext context);
    }
}
