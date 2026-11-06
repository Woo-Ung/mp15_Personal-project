using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerAction : MonoBehaviour, IStateObserver
{
    [SerializeField] private Canvas _attackRouteCanvas;
    [SerializeField] private List<PlayerAttackPoint> _pointList = new();
    private GameState _charaState;

    private int testnum;

    private void Start()
    {
        CacheComponents();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (_pointList != null)
            {
                if (testnum != 0)
                {
                    transform.position = _pointList[testnum]._transform.position;
                    transform.rotation = _pointList[testnum - 1]._transform.rotation;
                }

                testnum++;             
            }
        }
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

    }
    public void DeadState()
    {

    }
    public void ClearState()
    {

    }

}
