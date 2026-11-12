using UnityEngine;

public class BossController : EnemyController
{
    [SerializeField] private int _life;

    protected virtual void Start()
    {
        base.Start(); // 부모 클래스에 CacheComponents 등록됨
    }

    public override void Damaged()
    {
        _life -= 1;
        if(_life <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SpwanSword()
    {

    }

    protected override void PunishAttack()
    {

    }
    public override void ShootNoticeRay()
    {

    }
    public override void ShootAttackRay()
    {

    }
    public override void SetInRange(bool inR)
    {

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

    protected override void CacheComponents()
    {
        base.CacheComponents(); // 부모 클래스에서 옵저버 구독
    }

    //protected override void OnDestroy()
    //{
    //    StageManager.Instance.Unsubscribe(this);
    //}
}
