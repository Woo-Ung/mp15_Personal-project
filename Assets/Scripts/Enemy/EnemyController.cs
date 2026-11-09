using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public void Damaged()
    {
        Destroy(gameObject);
    }
}
