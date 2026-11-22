using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private float _moveSpeed = 5f;

    private Vector2 _moveInput;

    private void OnEnable() => BindInputEvents();

    private void Update() => Move();

    private void OnDisable() => UnbindInputEvents();

    private void BindInputEvents()
    {
        _moveAction.action.Enable();
        _moveAction.action.performed += OnMove;
        _moveAction.action.canceled += OnMove;
    }

    private void UnbindInputEvents()
    {
        _moveAction.action.Disable();
        _moveAction.action.performed -= OnMove;
        _moveAction.action.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);
        transform.Translate(direction * _moveSpeed * Time.deltaTime, Space.World);
    }
}
