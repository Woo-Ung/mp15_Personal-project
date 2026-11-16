using UnityEngine;
using System;

public class CameraInput : MonoBehaviour
{
    private CameraInputActions _cameraInput;

    private void Awake()
    {
        _cameraInput = new CameraInputActions();
        _cameraInput.Mouse.Enable();
    }

    private void OnDestroy()
    {
        _cameraInput.Mouse.Disable();
    }

    public Vector2 GetMousePosition()
    {
        return _cameraInput.Mouse.Move.ReadValue<Vector2>();
    }
}
