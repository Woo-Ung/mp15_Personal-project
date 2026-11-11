using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour, IDamageable, IStateObserver
{
    [field: SerializeField] public float _noticeRange { get; protected set; }

    public abstract void Damaged();
    protected abstract void PunishAttack();
    public abstract void ShootNoticeRay();
    public abstract void ShootAttackRay();
    public abstract void SetInRange(bool inR);

    protected virtual void Start()
    {
        CacheComponents();
    }

    public virtual void IdleState()
    {
        
    }
    public virtual void FocusState()
    {
        
    }
    public virtual void PunishState()
    {
       
    }
    public virtual void AttackState()
    {

    }
    public virtual void DeadState()
    {

    }
    public virtual void ClearState()
    {

    }

    protected virtual void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    protected virtual void OnDestroy()
    {
        StageManager.Instance.Unsubscribe(this);
    }
}
