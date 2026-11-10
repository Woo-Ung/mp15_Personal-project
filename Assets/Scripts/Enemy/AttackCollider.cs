using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    private SphereCollider _attackCollider => GetComponent<SphereCollider>();

    public void SetAttackCollider(float radius)
    {
        _attackCollider.radius = radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("AttackCol: Player감지");
            _enemyController.ShootAttackRay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("AttackCol: Player나감");
        }
    }
}
