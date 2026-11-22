using UnityEngine;

public class DestoryObj : MonoBehaviour, IDamageable
{
    public void Damaged()
    {
        GetComponent<TutorialBreakableTarget>()?.ReportDestroyed();
        Destroy(gameObject);
    }
}
