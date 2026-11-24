using System;
using UnityEngine;

public class CameraController : MonoBehaviour, IStateObserver
{
    [SerializeField] private Camera _charaCamera;
    [SerializeField] private LayerMask _hardMask;
    [SerializeField] private LayerMask _normalMask;

    private void Start()
    {
        CacheComponents();
        SetLevel();
    }

    private void SetLevel()
    {
        if (GameManager.Instance._level == Level.어려움)
        {
            _charaCamera.cullingMask = _hardMask;
        }
        else
        {
            _charaCamera.cullingMask = _normalMask;
        }
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
