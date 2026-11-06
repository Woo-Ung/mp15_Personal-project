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
    private WaitForSeconds _wait;
    private GameState _charaState;

    private int testnum;

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
        //if(Input.GetKeyDown(KeyCode.Mouse2))
        //{
        //    if (_pointList != null)
        //    {
        //        if (testnum != 0)
        //        {
        //            transform.position = _pointList[testnum]._transform.position;
        //            transform.rotation = _pointList[testnum - 1]._transform.rotation;
        //        }

        //        testnum++;             
        //    }
        //}
    }

    private void Init()
    {        
        _wait = new WaitForSeconds(_waitTime);
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
        testnum = 0;
    }

    public void AddAttackPoint(PlayerAttackPoint point)
    {
        _pointList.Add(point);
    }

    public void IdleState()
    { 
        //_attackRouteCanvas.gameObject.SetActive(false);

        _charaState = GameState.기본;
    }
    public void FocusState()
    {
        //_attackRouteCanvas.gameObject.SetActive(true);
        _charaState = GameState.집중;
        Instantiate(_attackRouteCanvas, transform.position, transform.rotation);        
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
        if (_pointList != null)
        {
            for (int i = 0; i < _pointList.Count; i++)
            {
                if (i > 0)
                {
                    transform.position = _pointList[i]._transform.position;
                    transform.rotation = _pointList[i - 1]._transform.rotation;
                }                
                yield return _wait;
                _waitTime -= 0.2f;
                _wait = new WaitForSeconds(_waitTime);
            }
        }        
    }
}
