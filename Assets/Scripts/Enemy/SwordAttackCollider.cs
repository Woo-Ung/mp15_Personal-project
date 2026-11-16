using System.Collections;
using UnityEngine;

public class SwordAttackCollider : MonoBehaviour
{
    private BoxCollider _attackCollider => GetComponent<BoxCollider>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StageManager.Instance.StateUpdate(GameState.사망);
            //StartCoroutine(DropAttack());
        }
    }

    //private IEnumerator DropAttack()
    //{
    //    yield return new WaitForSeconds(0.3f);
        
    //}
}
