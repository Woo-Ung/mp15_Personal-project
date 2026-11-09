using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable, IStateObserver
{
    [field:SerializeField] public bool canPunish { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        CacheComponents();
    }

    private void Init()
    {
        canPunish = false;
    }
    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    public void SetPunish(bool can)
    {
        canPunish = can;
    }

    private void PunishAttack()
    { 
        StageManager.Instance.StateUpdate(GameState.사망);
    }

    public void Damaged()
    {
        Destroy(gameObject);
    }     

    public void IdleState()
    {

    }
    public void FocusState()
    {

    }
    public void PunishState()
    {
        if (canPunish)
        {
            Debug.Log("공격가능");
            PunishAttack();
        }
    }
    public void AttackState()
    {

    }
    public void DeadState()
    {

    }
    public void ClearState()
    {

    }

    private void OnDestroy()
    {
        StageManager.Instance.Unsubscribe(this);
    }
}
