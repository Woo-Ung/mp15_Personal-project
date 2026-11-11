using System;
using System.Collections;
using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    //[field: SerializeField] public float _noticeRange { get; protected set; } 부모클래스에서 정의
    [field: SerializeField] public bool _isInRange { get; private set; }
  
    [SerializeField] private PlayerAction _player;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _setAttackRange;
    [SerializeField] private float _addAttackRange;
    [SerializeField] private AttackCollider _attackCollider;

    private EnemyMovement _enemyMovement;
    private Quaternion _prevRotation;


    public float _attackRange { get; private set; } 

    private void Awake()
    {
        Init();
    }

    protected override void Start()
    {
        base.Start(); // 부모 클래스에 CacheComponents 등록됨
    }

    private void Init()
    {       
        _isInRange = false;
        _attackRange = _setAttackRange;
        _enemyMovement = transform?.GetComponent<EnemyMovement>();
    }
    protected override void CacheComponents()
    {
        base.CacheComponents(); // 부모 클래스에서 옵저버 구독함
    }

    private IEnumerator SetAttackRange()
    {
        while (_isInRange)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                _attackRange = Mathf.Min(_attackRange + Time.deltaTime * _addAttackRange, _noticeRange);
                _attackCollider.SetAttackCollider(_attackRange);
            }
            yield return null;
        }
        _attackRange = _setAttackRange;
        _attackCollider.SetAttackCollider(_attackRange);
    }

    public override void SetInRange(bool inR)
    {
        if(_isInRange == inR)
        {
            return;
        }

        if(inR)
        {
            _prevRotation = transform.rotation;
            _isInRange = true;
            LookPlayer();
        }

        else
        {
            _isInRange = false;
            transform.rotation = _prevRotation;
        }

        bool canMove = (StageManager.Instance.PlayerState == GameState.기본 && !_isInRange);

        _enemyMovement?.SetCanMove(canMove);
    }

    protected override void PunishAttack()
    {
        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z); // Ray 나가는 높이 조절
        Vector3 target = _player.transform.position + Vector3.up;
        Vector3 direction = (target - origin).normalized;

        transform.position = target + (direction * 1.2f);

        StageManager.Instance.StateUpdate(GameState.사망);
    }

    public override void Damaged()
    {
        Destroy(gameObject);
    }     

    public override void ShootAttackRay()
    {
        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z); // Ray 나가는 높이 조절
        Vector3 target = _player.transform.position + Vector3.up;

        Vector3 direction = (target - origin).normalized;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction * (_attackRange + 0.05f), Color.blue, 5f);

        if (Physics.Raycast(ray, out RaycastHit hit, (_attackRange + 0.05f), _layerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("AttackRay: 타겟확인");
                PunishAttack();
            }
            else
            {
                Debug.Log($"AttackRay: 타겟놓침:{hit.transform.name}");
            }
        }
    }

    public override void ShootNoticeRay()
    {
        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z); // Ray 나가는 높이 조절
        Vector3 target = _player.transform.position + Vector3.up;

        Vector3 direction = (target - origin).normalized;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction * (_noticeRange + 0.05f), Color.red, 5f);

        if (Physics.Raycast(ray, out RaycastHit hit, (_noticeRange + 0.05f), _layerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("NoticeRay: 타겟감지");
                SetInRange(true);
                StartCoroutine(SetAttackRange());
            }
            else
            {
                Debug.Log("NoticeRay: 타겟안보임");
                SetInRange(false);
            }
        }
    }

    private void LookPlayer()
    {
        if (_player.transform != null)
        {
            transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
        }
    }

    public override void IdleState()
    {
        _enemyMovement?.SetCanMove(!_isInRange);
    }
    public override void FocusState()
    {
        _enemyMovement?.SetCanMove(false);
    }
    public override void PunishState()
    {
        ShootAttackRay();               
    }
    //public override void AttackState()
    //{

    //}
    //public override void DeadState()
    //{

    //}
    //public override void ClearState()
    //{

    //}       

    //protected override void OnDestroy()
    //{
    //    base.OnDestroy();
    //}
}
