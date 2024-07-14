using System;
using UnityEngine;

public class GameInputs : MonoBehaviour
{

    public static GameInputs Instance { get; private set; }

    public event EventHandler OnLeftMouseDown;
    public event EventHandler OnLeftMouseRelease;

    private PlayerInputActions inputActions;

    private void Awake() {
        Instance = this;
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.Player.MouseRotate.started += MouseRotate_started;
        inputActions.Player.MouseRotate.canceled += MouseRotate_canceled;
    }

    private void MouseRotate_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnLeftMouseDown?.Invoke(this, EventArgs.Empty);
    }

    private void MouseRotate_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnLeftMouseRelease?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetKeyRotate() {
        return inputActions.Player.KeyRotate.ReadValue<Vector2>();
    }
}
