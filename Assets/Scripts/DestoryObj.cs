using UnityEngine;

public class DestoryObj : MonoBehaviour, IDamageable
{
    [SerializeField] private bool _isClearTarget = false;

    public void Damaged()
    {
        GetComponent<TutorialBreakableTarget>()?.ReportDestroyed();

        if (_isClearTarget)
        {
            StageManager.Instance.StateUpdate(GameState.클리어);
        }

        Destroy(gameObject);
    }
}
