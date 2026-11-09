using UnityEngine;

public class RunningCheck : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }
}