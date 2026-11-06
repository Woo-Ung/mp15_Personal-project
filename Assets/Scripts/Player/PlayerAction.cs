using UnityEngine;

public class PlayerAction : MonoBehaviour, IStateObserver
{
    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    public void IdleState()
    {

    }
    public void FocusState()
    {

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
