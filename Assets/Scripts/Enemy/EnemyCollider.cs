using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController; 
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player감지");
            _enemyController.SetPunish(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player나감");
            _enemyController.SetPunish(false);
        }
    }
}
