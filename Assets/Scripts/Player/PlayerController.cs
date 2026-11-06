using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameState _PlayerState;
    [SerializeField] private InputActionReference _focusAction;
    [SerializeField] private InputActionReference _directionAction;
    [SerializeField] private InputActionReference _distanceAction;
    [SerializeField] private InputActionReference _undoAction;
    
    private void Start()
    {
        
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
        _directionAction.action.Enable();
        _distanceAction.action.Enable();
        _undoAction.action.Enable();
    }

    private void OffInputAction()
    {
        _focusAction.action.Disable();
        _directionAction.action.Disable();
        _distanceAction.action.Disable();
        _undoAction.action.Disable();
    }

    private void BindInputEvent()
    {
        _focusAction.action.performed += FocusMode;
        //_undoAction.action.performed += Undo;
    }

    private void UnbindInputEvent()
    {
        _focusAction.action.performed -= FocusMode;
        //_undoAction.action.performed -= Undo;
    }

    private void FocusMode(InputAction.CallbackContext context)
    {
        Debug.Log("집중모드");
        StageManager.Instance.StateUpdate(GameState.집중);
    }

    //private void Undo(InputAction.CallbackContext context)
    //{
    //    Debug.Log("취소");
    //    _stageManager.OffFocusMode();
    //}
}
