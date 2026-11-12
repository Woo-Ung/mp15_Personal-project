using UnityEngine;

public class DestoryObj : MonoBehaviour, IDamageable
{
    public void Damaged()
    {
        Destroy(gameObject);
    }
}
