using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour, IStateObserver
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private Canvas _attackRouteCanvas;
    [SerializeField] private List<PlayerAttackPoint> _pointList = new();
    [SerializeField] private float _waitTime;    
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _timeValue;
    [SerializeField] private GameObject _eye;
    [SerializeField] private Image _eyeFire;

    [SerializeField] private GameObject _Idle;
    [SerializeField] private GameObject[] _Attack;

    private WaitForSeconds _wait;
    private Coroutine _attackCoroutine;


    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        CacheComponents();
        if (_camera != null)
        {
            _camera.transform.position = transform.position;
        }
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        StageManager.Instance.StateUpdate(GameState.기본);
    }

    private void Init()
    {        
        _wait = new WaitForSeconds(_waitTime);
        for (int i = 0; i < _Attack.Length; i++)
        {
            _Attack[i].SetActive(false);
        }
        _eyeFire.gameObject.SetActive(false);
        _eye.SetActive(false);
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    public void AddAttackPoint(PlayerAttackPoint point)
    {
        _pointList.Add(point);
    }

    private void IdlePose()
    {
        for (int i = 0; i < _Attack.Length; i++)
        {
            _Attack[i].SetActive(false);
        }
        _Idle.SetActive(true);
    }

    private void AttackPose()
    {
        _Idle.SetActive(false);
        for (int i = 0; i < _Attack.Length; i++)
        {
            _Attack[i].SetActive(false);
        }
        int num = Random.Range(0, _Attack.Length);

        _Attack[num].SetActive(true);

    }

    public void IdleState()
    {
        IdlePose();

        _eye.SetActive(false);
        _eyeFire.gameObject.SetActive(false);
    }
    public void FocusState()
    {
        TutorialManager.Instance?.OnFocusEntered(); // 튜토리얼용
        Instantiate(_attackRouteCanvas, new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);
        _eye.SetActive(true);
        _eyeFire.gameObject.SetActive(true);

        _eyeFire.DOKill();
        _eyeFire.fillAmount = 0f;

        _eyeFire.DOFillAmount(1f, 0.5f)
                .SetEase(Ease.OutQuad);
    }
    
    public void PunishState()
    {

    }

    public void AttackState()
    {
        _eye.SetActive(false);
        _eyeFire.gameObject.SetActive(false);

        if (_pointList.Count <= 1)
        {
            ResetAttack();
            StageManager.Instance.StateUpdate(GameState.기본);
            return;
        }

        TutorialManager.Instance?.OnAttackStarted(); // 튜토리얼용
        _attackCoroutine = StartCoroutine(Attack());
    }
    public void DeadState()
    {
        _eye.SetActive(false);
        _eyeFire.gameObject.SetActive(false);

        StopAllCoroutines();
        ResetAttack();
        Debug.Log("죽음ㅠㅠ");

        StartCoroutine(TimeStop());       
    }

    public void ClearState()
    {

    }

    private IEnumerator TimeStop()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
    }

    private IEnumerator Attack()
    {

        if (_pointList.Count > 0)
        {
            yield return _wait;
            for (int i = 1; i < _pointList.Count; i++)
            {
                ScanRoute(_pointList[i]._transform.position, out Vector3 hitPosition, out bool isWallExist, out RaycastHit wall);

                ScanEnemy(hitPosition, out RaycastHit[] hits);
                transform.position = hitPosition;
                transform.rotation = _pointList[i - 1]._transform.rotation;
                Physics.SyncTransforms(); // 물리좌표 동기화

                if (hits.Length > 0)
                {
                    AttackPose();
                    for (int j = 0; j < hits.Length; j++)
                    {
                        hits[j].transform.GetComponentInParent<IDamageable>().Damaged();
                    }
                }
                else
                {
                    IdleState();
                    yield return new WaitForSeconds(0.05f);
                    StageManager.Instance.StateUpdate(GameState.응징);
                    yield return new WaitForSeconds(0.05f);
                }

                if (!isWallExist)
                {
                    if (!(i == _pointList.Count - 1))
                    {
                        yield return _wait;
                        _waitTime -= 0.2f;
                        _waitTime = Mathf.Max(_waitTime, 0.1f);
                        _wait = new WaitForSeconds(_waitTime);
                    }
                }
                
                else
                {
                    if(wall.transform.TryGetComponent<IDamageable>(out IDamageable damaged))
                    {
                        damaged.Damaged();
                    }
                    IdleState();
                    break;
                }
            }
            yield return new WaitForSeconds(0.3f);

            ResetAttack();
            TutorialManager.Instance?.OnAttackFinished(); // 튜토리얼용
        }

        StageManager.Instance.StateUpdate(GameState.기본);
    }

    public void Undo()
    {
        if(StageManager.Instance.PlayerState != GameState.집중)
        {
            return;
        }
        if (_pointList.Count <= 1)
        {
            return;
        }
        StageManager.Instance.FocusTimeUpdate(_timeValue);
        Destroy(_pointList[(_pointList.Count - 1)]._transform.gameObject);
        _pointList.RemoveAt(_pointList.Count - 1);
        _pointList[(_pointList.Count - 1)]._attackPoint.GetComponent<AttackRouteUI>().CanSelect();
        
    }

    private void ResetAttack()
    {
        if (_pointList.Count < 1)
        {
            return;
        }

        for (int i = 0; i < _pointList.Count; i++)
        {
            Destroy(_pointList[i]._transform.gameObject);
        }

        _waitTime = 1f;
        _pointList.Clear();
    }

    private void ScanRoute(Vector3 nextPoint, out Vector3 hitPosition, out bool isWallExist, out RaycastHit wall)
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 1f, 0), (nextPoint - transform.position).normalized);
        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), (nextPoint - transform.position), Color.red, 5f);
        float distance = Vector3.Distance(transform.position, nextPoint);

        if(Physics.Raycast(ray,out RaycastHit hit, distance, _wallLayer))
        {
            hitPosition = hit.point + hit.normal * 0.7f; //벽의 표면에서 0.7f만큼 떨어진 위치
            hitPosition = new Vector3(hitPosition.x, transform.position.y, hitPosition.z);
            wall = hit;
            isWallExist = true;
            //Debug.Log("장애물있음");
        }
        else
        {
            isWallExist = false;
            wall = default;
            hitPosition = nextPoint;
            //Debug.Log("장애물감지못함");
        }        
    }

    public void StopAttack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        ResetAttack();
    }

    private void ScanEnemy(Vector3 nextPoint, out RaycastHit[] hits)
    {
        float distance = Vector3.Distance(transform.position, nextPoint);
        hits = Physics.SphereCastAll(transform.position, 0.6f, ((nextPoint - transform.position).normalized), distance, _enemyLayer);
        if (hits.Length > 0)
        {
            //Debug.Log($"적 수:{hits.Length}");
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].transform.name);
            }
        }        
    }
}