using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerAction : MonoBehaviour, IStateObserver
{
    [SerializeField] private Canvas _attackRouteCanvas;
    [SerializeField] private List<PlayerAttackPoint> _pointList = new();
    [SerializeField] private float _waitTime;
    //[SerializeField] private Transform _rayPoint;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _enemyLayer;
    private WaitForSeconds _wait;


    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        CacheComponents();
    }

    private void Update()
    {
        
    }

    private void Init()
    {        
        _wait = new WaitForSeconds(_waitTime);
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    public void AddAttackPoint(PlayerAttackPoint point)
    {
        _pointList.Add(point);
    }

    public void IdleState()
    {
        
    }
    public void FocusState()
    {        
        Instantiate(_attackRouteCanvas, new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);        
    }
    
    public void PunishState()
    {

    }

    public void AttackState()
    {
        if (_pointList.Count <= 1)
        {
            ResetAttack();
            StageManager.Instance.StateUpdate(GameState.기본);
            return;
        }

        StartCoroutine(Attack());
    }
    public void DeadState()
    {
        StopAllCoroutines();
        Debug.Log("죽음ㅠㅠ");
        Time.timeScale = 0f;
    }
    public void ClearState()
    {

    }

    private IEnumerator Attack()
    {
        if (_pointList.Count > 0)
        {
            yield return _wait;
            for (int i = 1; i < _pointList.Count; i++)
            {
                ScanRoute(_pointList[i]._transform.position, out Vector3 hitPosition, out bool isWallExit);

                ScanEnemy(hitPosition, out RaycastHit[] hits);
                transform.position = hitPosition;
                transform.rotation = _pointList[i - 1]._transform.rotation;

                if (hits.Length > 0)
                {
                    for (int j = 0; j < hits.Length; j++)
                    {
                        hits[j].transform.GetComponent<IDamageable>().Damaged();
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                    StageManager.Instance.StateUpdate(GameState.응징);
                    yield return new WaitForSeconds(0.05f);
                }

                if (!isWallExit)
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
                    break;
                }               
            }

            ResetAttack();
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

    private void ScanRoute(Vector3 nextPoint, out Vector3 hitPosition, out bool isWallExit)
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 1f, 0), (nextPoint - transform.position).normalized);
        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), (nextPoint - transform.position), Color.red, 5f);
        float distance = Vector3.Distance(transform.position, nextPoint);

        if(Physics.Raycast(ray,out RaycastHit hit, distance, _wallLayer))
        {
            hitPosition = hit.point + hit.normal * 0.7f; //벽의 표면에서 0.7f만큼 떨어진 위치
            hitPosition = new Vector3(hitPosition.x, transform.position.y, hitPosition.z);
            isWallExit = true;
            Debug.Log("장애물있음");
        }
        else
        {
            isWallExit = false;
            hitPosition = nextPoint;
        }        
    }

    private void ScanEnemy(Vector3 nextPoint, out RaycastHit[] hits)
    {
        float distance = Vector3.Distance(transform.position, nextPoint);
        hits = Physics.SphereCastAll(transform.position, 0.5f, ((nextPoint - transform.position).normalized), distance, _enemyLayer);
        if (hits.Length > 0)
        {
            Debug.Log($"적 수:{hits.Length}");
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].transform.name);
            }
        }        
    }
}