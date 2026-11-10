using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyController : MonoBehaviour, IDamageable, IStateObserver
{
    [field: SerializeField] public bool _isInRange { get; private set; }
    [field: SerializeField] public float _noticeRange { get; private set; }
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

    private void Start()
    {
        CacheComponents();
    }

    private void Init()
    {       
        _isInRange = false;
        _attackRange = _setAttackRange;
        _enemyMovement = transform?.GetComponent<EnemyMovement>();
    }
    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
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

    public void SetInRange(bool inR)
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

    private void PunishAttack()
    { 
        StageManager.Instance.StateUpdate(GameState.사망);
    }

    public void Damaged()
    {
        Destroy(gameObject);
    }     

    public void ShootAttackRay()
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

    public void ShootNoticeRay()
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

    public void IdleState()
    {
        _enemyMovement?.SetCanMove(!_isInRange);
    }
    public void FocusState()
    {
        _enemyMovement?.SetCanMove(false);
    }
    public void PunishState()
    {
        ShootAttackRay();               
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
