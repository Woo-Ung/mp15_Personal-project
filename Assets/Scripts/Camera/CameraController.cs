using System;
using UnityEngine;

public class CameraController : MonoBehaviour, IStateObserver
{
    [SerializeField] private Camera _charaCamera;

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    public void IdleState()
    {
        _charaCamera.gameObject.SetActive(false);
    }
    public void FocusState()
    {
        _charaCamera.gameObject.SetActive(true);
    }
    public void PunishState()
    {
        _charaCamera.gameObject.SetActive(false);
    }
    public void AttackState()
    {
        _charaCamera.gameObject.SetActive(false);
    }
    public void DeadState()
    {
        _charaCamera.gameObject.SetActive(false);
    }
    public void ClearState()
    {
        _charaCamera.gameObject.SetActive(false);
    }
}
