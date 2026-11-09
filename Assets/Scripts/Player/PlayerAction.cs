using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerAction : MonoBehaviour, IStateObserver
{
    [SerializeField] private Canvas _attackRouteCanvas;
    [SerializeField] private List<PlayerAttackPoint> _pointList = new();
    [SerializeField] private float _waitTime;
    //[SerializeField] private Transform _rayPoint;
    [SerializeField] private LayerMask _wallLayer;
    private WaitForSeconds _wait;
    private GameState _charaState;


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
        _charaState = GameState.기본;
    }
    public void FocusState()
    {
        _charaState = GameState.집중;
        Instantiate(_attackRouteCanvas, transform.position + new Vector3(0,0.1f,0), transform.rotation);        
    }
    
    public void PunishState()
    {

    }

    public void AttackState()
    {
        StartCoroutine(Attack());
        
    }
    public void DeadState()
    {

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
                ScanRoute(_pointList[i]._transform, out Vector3 hitPosition);

                if (_pointList[i]._transform.position == hitPosition)
                {

                    transform.position = _pointList[i]._transform.position;
                    transform.rotation = _pointList[i - 1]._transform.rotation;

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
                    transform.position = hitPosition;
                    transform.rotation = _pointList[i - 1]._transform.rotation;
                    break;
                }
            }
            for (int i = 0; i < _pointList.Count; i++)
            {
                Destroy(_pointList[i]._transform.gameObject);
            }
            ResetAttack();
        }
    }

    private void ResetAttack()
    {
        _waitTime = 1f;
        _pointList.Clear();
    }

    private void ScanRoute(Transform nextPoint, out Vector3 hitPosition)
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 1f, 0), (nextPoint.position - transform.position).normalized);
        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), (nextPoint.position - transform.position), Color.red, 5f);
        float distance = Vector3.Distance(transform.position, nextPoint.position);

        if(Physics.Raycast(ray,out RaycastHit hit, distance, _wallLayer))
        {
            hitPosition = hit.point + hit.normal * 0.7f; //벽의 표면에서 0.7f만큼 떨어진 위치
            hitPosition = new Vector3(hitPosition.x, transform.position.y, hitPosition.z);
            Debug.Log("장애물있음");
        }
        else
        {
            hitPosition = nextPoint.position;
        }        
    }
}
