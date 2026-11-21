using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MeleeEnemyController : EnemyController
{
    //[field: SerializeField] public float _noticeRange { get; protected set; } 부모클래스에서 정의
    [field: SerializeField] public bool _isInRange { get; private set; }
  
    [SerializeField] private PlayerAction _player;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _setAttackRange;
    [SerializeField] private float _addAttackRange;
    [SerializeField] private AttackCollider _attackCollider;
    [SerializeField] private float _returnTime;
    [SerializeField] private Image _focusMark;    
    [SerializeField] private GameObject _idle;
    [SerializeField] private GameObject _attackReady;
    [SerializeField] private GameObject _attack;

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
        _focusMark.gameObject.SetActive(false);
        _attack.SetActive(false);
        _attackReady.SetActive(false);
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

            _idle.SetActive(false);
            _attackReady.SetActive(true);
            _attack.SetActive(false);
        }

        else
        {
            _isInRange = false;
            transform.rotation = _prevRotation;

            _idle.SetActive(true);
            _attackReady.SetActive(false);
            _attack.SetActive(false);
        }

        bool canMove = (StageManager.Instance.PlayerState == GameState.기본 && !_isInRange);

        _enemyMovement?.SetCanMove(canMove);
    }

    protected override void PunishAttack()
    {
        _attack.SetActive(true);
        _attackReady.SetActive(false);
        _idle.SetActive(false);

        StageManager.Instance.StateUpdate(GameState.사망);
                
        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
        Vector3 target = _player.transform.position + Vector3.up;
        Vector3 direction = (target - origin).normalized;

        transform.position = target + (direction * 1.2f);        
    }

    public override void Damaged()
    {
        StageManager.Instance.FocusTimeUpdate(_returnTime);
        Destroy(gameObject);
    }     

    public override void ShootAttackRay()
    {
        if(StageManager.Instance.PlayerState == GameState.공격)
        {
            return;
        }

        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z); // Ray 나가는 높이 조절
        Vector3 target = _player.transform.position + Vector3.up;

        Vector3 direction = (target - origin).normalized;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction * (_attackRange + 0.05f), Color.blue, 5f);

        if (Physics.Raycast(ray, out RaycastHit hit, (_attackRange + 0.05f), _layerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //Debug.Log("AttackRay: 타겟확인");
                PunishAttack();
            }
            else
            {
                //Debug.Log($"AttackRay: 타겟놓침:{hit.transform.name}");
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
                //Debug.Log("NoticeRay: 타겟감지");
                SetInRange(true);
                StartCoroutine(SetAttackRange());
            }
            else
            {
                //Debug.Log("NoticeRay: 타겟안보임");
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
        _focusMark.gameObject.SetActive(false);

        ShootAttackRay();
    }
    public override void FocusState()
    {
        _enemyMovement?.SetCanMove(false);
        _focusMark.gameObject.SetActive(true);
        _focusMark.rectTransform.LookAt(Camera.main.transform);
        _focusMark.DOFade(1f, 0.01f);
        _focusMark.DOFade(0f, 1f).SetEase(Ease.InQuint);        
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
