using UnityEngine;

public class PlayerAttackPoint:MonoBehaviour
{
    public Transform _transform { get; private set; }

    public void SetPosition(Transform transform)
    {
        _transform = transform;
    }

}
