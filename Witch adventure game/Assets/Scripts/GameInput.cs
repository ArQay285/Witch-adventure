using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {

    public static GameInput Instance { get; private set; }

    public event EventHandler OnPlayerAttack;

    public event EventHandler OnPlayerDash;

    private PlayerInputActions _playerInputActions;
    private void Awake() {
        Instance = this;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _playerInputActions.Combat.Attack.started += PlayerAttack_started;
        _playerInputActions.Player.Dash.performed += PlayerDash_performed;
    }

    public void DisableMovement() {
        _playerInputActions.Player.Disable();
    }

    public Vector2 GetMovementVector() {
        Vector2 movement = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return movement;
    }
    public Vector2 GetMousePosition() {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

    private void PlayerDash_performed(InputAction.CallbackContext obj) {
        if (Player.Instance.IsAlive())
            OnPlayerDash?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerAttack_started(InputAction.CallbackContext obj) {
        if (Player.Instance.IsAlive())
            OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }

}
