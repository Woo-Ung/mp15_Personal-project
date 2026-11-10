using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    private SphereCollider _attackCollider => GetComponent<SphereCollider>();

    private void Update()
    {
        //if(!_enemyController._isInRange)
        //{
        //    return;
        //}
        _attackCollider.radius = _enemyController._attackRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Attack: Player감지");
            _enemyController.ShootAttackRay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Attack: Player나감");
        }
    }
}
