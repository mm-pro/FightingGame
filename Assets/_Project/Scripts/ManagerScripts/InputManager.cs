using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnInputChange;
    [SerializeField] private PlayerInput _playerInput;

    public PromptsInput CurrentPrompts;
    public Vector2 NavigationInput { get; private set; }
    public string InputScheme { get { return _playerInput.devices[0].displayName; } set { } }
    public PlayerInput PlayerInput { get { return _playerInput; } private set { } }

    public void InputChange(PlayerInput playerInput)
    {
        OnInputChange?.Invoke();
    }

    public void Navigation(CallbackContext callbackContext)
    {
        NavigationInput = callbackContext.ReadValue<Vector2>();
    }

    public void Confirm(CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            CurrentPrompts?.OnConfirm?.Invoke();
        }
    }

    public void Back(CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            CurrentPrompts?.OnBack?.Invoke();
        }
    }

    public void Stage(CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            CurrentPrompts?.OnStage?.Invoke();
        }
    }

    public void Coop(CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            CurrentPrompts?.OnCoop?.Invoke();
        }
    }

    public void Rebind(CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            CurrentPrompts?.OnRebind?.Invoke();
        }
    }


    public void Controls(CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            CurrentPrompts?.OnControls?.Invoke();
        }
    }

    public void ToggleFramedata(CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            CurrentPrompts?.OnToggleFramedata?.Invoke();
        }
    }
}
