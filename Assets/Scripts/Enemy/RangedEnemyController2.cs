using UnityEngine;

public class RangedEnemyController2 : MonoBehaviour, IDamageable, IStateObserver
{
    [field: SerializeField] public bool canPunish { get; private set; }
    [SerializeField] private PlayerAction _player;
    [SerializeField] private LayerMask _layerMask;

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
        if (_player.transform != null)
        {
            transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
        }

    }
    public void FocusState()
    {

    }
    public void PunishState()
    {
        if (_player.transform != null)
        {
            transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
        }

        Ray ray = new Ray(transform.position + new Vector3(0, 1f, 0), (_player.transform.position - transform.position).normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, 300f, _layerMask))
        {
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("타겟확인");
                canPunish = true;
            }
            else 
            {
                Debug.Log("타겟놓침");
                canPunish = false;
            }
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
