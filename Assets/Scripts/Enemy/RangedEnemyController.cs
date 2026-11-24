using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RangedEnemyController : EnemyController
{
    //[field: SerializeField] public float _noticeRange { get; protected set; } 부모클래스에서 정의
    [field: SerializeField] public bool _isInRange { get; private set; }
    [SerializeField] private PlayerAction _player;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _attackCoolTime;
    [SerializeField] private LineRendererAtoB _attackLine;
    [SerializeField] private float _returnTime;
    [SerializeField] private GameObject _idle;
    [SerializeField] private GameObject _attack;
    [SerializeField] private Image _focusMark;
    [SerializeField] private Image _punishMark;

    private EnemyMovement _enemyMovement;
    private Quaternion _prevRotation;
    private Coroutine _attackCoroutine;

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
        _attackRange = _noticeRange;
        _enemyMovement = transform?.GetComponent<EnemyMovement>();
        _focusMark.gameObject.SetActive(false);
        _punishMark.gameObject.SetActive(false);
        _idle.SetActive(true);
        _attack.SetActive(false);
    }

    protected override void CacheComponents()
    {
        base.CacheComponents(); // 부모 클래스에서 옵저버 구독함
    }

    private IEnumerator StartAttackCoolTime()
    {
        float currentTime = 0f;
        float width = 0f;

        while (_isInRange)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime += Time.deltaTime;
                width = Mathf.Lerp(0f, 0.2f, currentTime / _attackCoolTime);
                _attackLine.SetLineWidth(width);
                if (currentTime > _attackCoolTime)
                {
                    PunishAttack();
                    break;
                }
            }
            yield return null;
        }
        _attackLine.SetLineWidth(0);
        _attackLine.Stop();
    }

    public override void SetInRange(bool inR)
    {
        if (_isInRange == inR)
        {
            return;
        }

        if (inR)
        {
            _prevRotation = transform.rotation;
            _isInRange = true;

            _idle.SetActive(false);
            _attack.SetActive(true);            
        }

        else
        {
            _isInRange = false;

            _idle.SetActive(true);
            _attack.SetActive(false);

            transform.rotation = _prevRotation;
        }

        bool canMove = (StageManager.Instance.PlayerState == GameState.기본) && !_isInRange;

        _enemyMovement?.SetCanMove(canMove);
    }

    protected override void PunishAttack()
    {        
        LookPlayer();
        _idle.SetActive(false);
        _attack.SetActive(true);
        StageManager.Instance.StateUpdate(GameState.사망);
        CameraController _mainCam = Camera.main.GetComponentInParent<CameraController>();
        _punishMark.gameObject.SetActive(true);
        _mainCam.transform.position = transform.position;
    }

    public override void Damaged()
    {
        GetComponent<TutorialEnemyTarget>()?.ReportDefeated(); // 튜토리얼용
        StageManager.Instance.FocusTimeUpdate(_returnTime);
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
                //Debug.Log("AttackRay: 타겟확인");
                if (_attackCoroutine != null)
                {
                    StopCoroutine(_attackCoroutine);
                    _attackLine.Stop();
                    _attackCoroutine = null;
                }
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
                Debug.Log($"궁병 타겟감지aaaaaaaaaaaaaaaaaaa");                
                SetInRange(true);

                if(_attackCoroutine != null)
                {
                    StopCoroutine(_attackCoroutine);
                    _attackLine.Stop();
                    _attackCoroutine = null;
                }
                LookPlayer();
                _attackLine.Play(origin, target);
                _attackCoroutine = StartCoroutine(StartAttackCoolTime());
            }

            else
            {
                //Debug.Log($"{transform.name}_NoticeRay: 타겟안보임");
                SetInRange(false);
            }
        }
    }

    private void LookPlayer()
    {
        if (_player.transform != null)
        {
            _idle.SetActive(false);
            _attack.SetActive(true);
            transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
        }
    }

    public override void IdleState()
    {        
        _enemyMovement?.SetCanMove(!_isInRange);
        ShootNoticeRay();
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