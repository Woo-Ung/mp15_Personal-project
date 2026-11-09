using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{    
    private List<IStateObserver> _subscriber = new();
    public static StageManager Instance { get; private set; }
    [field: SerializeField] public GameState PlayerState { get; private set; }

    private void Awake()
    {
        SetSingleton();
        Init();
    }

    private void Start()
    {
        CasheComponent();
    }

    private void Init()
    {

    }       

    private void CasheComponent()
    {
        StateUpdate(GameState.기본);
    }

    public void Subscribe(IStateObserver observer)
    {
        _subscriber.Add(observer);
    }

    public void Unsubscribe(IStateObserver observer)
    {
        _subscriber.Remove(observer);
    }

    public void StateUpdate(GameState state)
    {
        switch (state)
        {
            case GameState.기본:
                Debug.Log("기본상태");
                for (int i = 0; i < _subscriber.Count; i++)
                {                    
                    PlayerState = GameState.기본;
                    _subscriber[i].IdleState();
                }
                break;
            case GameState.집중:
                if (PlayerState == GameState.기본)
                {
                    Debug.Log("집중상태");
                    for (int i = 0; i < _subscriber.Count; i++)
                    {
                        PlayerState = GameState.집중;
                        _subscriber[i].FocusState();
                    }
                }
                break;
            case GameState.공격:
                if (PlayerState == GameState.집중)
                {                  
                    Debug.Log("공격상태");
                    for (int i = 0; i < _subscriber.Count; i++)
                    {
                        PlayerState = GameState.공격;
                        _subscriber[i].AttackState();
                    }
                }
                break;
            case GameState.응징:
                Debug.Log("응징상태");
                for (int i = 0; i < _subscriber.Count; i++)
                {
                    PlayerState = GameState.응징;
                    _subscriber[i].PunishState();
                }
                break;
            case GameState.사망:
                for (int i = 0; i < _subscriber.Count; i++)
                {
                    Debug.Log("사망상태");
                    PlayerState = GameState.사망;
                    _subscriber[i].DeadState();
                }
                break;
            case GameState.클리어:
                for (int i = 0; i < _subscriber.Count; i++)
                {
                    Debug.Log("클리어");
                    PlayerState = GameState.클리어;
                    _subscriber[i].ClearState();
                }
                break;
        }
    }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

}
