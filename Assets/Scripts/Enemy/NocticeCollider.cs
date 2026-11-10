using UnityEngine;

public class NocticeCollider : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    private SphereCollider _noticeCollider => GetComponent<SphereCollider>();

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        _noticeCollider.radius = _enemyController._noticeRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Notice: Player감지");
            _enemyController.ShootNoticeRay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Notice: Player나감");
            _enemyController.SetInRange(false);
        }
    }
}