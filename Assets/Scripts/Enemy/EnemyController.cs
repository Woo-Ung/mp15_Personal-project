using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable, IStateObserver
{
    public bool canPunish { get; private set; }

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

    private IEnumerator PunishAttack()
    { 
        StageManager.Instance.StateUpdate(GameState.사망);
        yield return new WaitForSeconds(3f);
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
            StartCoroutine(PunishAttack());
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
