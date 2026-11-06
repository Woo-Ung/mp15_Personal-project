using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{    
    private List<IStateObserver> _subscriber = new();

    public static StageManager Instance { get; private set; }

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

    public void StateUpdate(GameState state)
    {
        switch (state)
        {
            case GameState.기본:
                for(int i = 0; i < _subscriber.Count; i++)
                {
                    _subscriber[i].IdleState();
                }
                break;
            case GameState.집중:
                for (int i = 0; i < _subscriber.Count; i++)
                {
                    _subscriber[i].FocusState();
                }
                break;
            case GameState.공격:
                for (int i = 0; i < _subscriber.Count; i++)
                {
                    _subscriber[i].AttackState();
                }
                break;
            case GameState.응징:
                for (int i = 0; i < _subscriber.Count; i++)
                {
                    _subscriber[i].PunishState();
                }
                break;
            case GameState.사망:
                for (int i = 0; i < _subscriber.Count; i++)
                {
                    _subscriber[i].DeadState();
                }
                break;
            case GameState.클리어:
                for (int i = 0; i < _subscriber.Count; i++)
                {
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
