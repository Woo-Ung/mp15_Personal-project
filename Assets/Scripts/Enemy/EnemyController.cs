using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyController : MonoBehaviour, IDamageable, IStateObserver
{
    [field:SerializeField] public bool canPunish { get; private set; }
    [SerializeField] private PlayerAction _player;

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
        if (_player.transform != null)
        {
            transform.LookAt(new Vector3(_player.transform.position.x,transform.position.y,_player.transform.position.z));
        }
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
