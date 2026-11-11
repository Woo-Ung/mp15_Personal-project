using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference _focusAction;
    [SerializeField] private InputActionReference _attackAction;
    [SerializeField] private InputActionReference _directionAction;
    [SerializeField] private InputActionReference _distanceAction;
    [SerializeField] private InputActionReference _undoAction;

    private PlayerAction _playerAction;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _playerAction = GetComponent<PlayerAction>();
    }

    private void OnEnable()
    {
        OnInputAction();
        BindInputEvent();
    }
    private void OnDisable()
    {
        OffInputAction();
        UnbindInputEvent();
    }       

    private void OnInputAction()
    {
        _focusAction.action.Enable();
        _attackAction.action.Enable();
        _directionAction.action.Enable();
        _distanceAction.action.Enable();
        _undoAction.action.Enable();
    }

    private void OffInputAction()
    {
        _focusAction.action.Disable();
        _attackAction.action.Disable();
        _directionAction.action.Disable();
        _distanceAction.action.Disable();
        _undoAction.action.Disable();
    }

    private void BindInputEvent()
    {
        _focusAction.action.performed += FocusState;
        _attackAction.action.performed += AttackState;
        _undoAction.action.performed += Undo;
    }

    private void UnbindInputEvent()
    {
        _focusAction.action.performed -= FocusState;
        _attackAction.action.performed -= AttackState;
        _undoAction.action.performed -= Undo;
    }

    private void FocusState(InputAction.CallbackContext context)
    {        
        StageManager.Instance.StateUpdate(GameState.집중);
    }
    
    private void AttackState(InputAction.CallbackContext context)
    {
        StageManager.Instance.StateUpdate(GameState.공격);
    }

    private void Undo(InputAction.CallbackContext context)
    {
        _playerAction.Undo();
    }
}
