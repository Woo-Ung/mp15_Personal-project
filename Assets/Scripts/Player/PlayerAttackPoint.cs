using UnityEngine;

public class PlayerAttackPoint:MonoBehaviour
{
    public Transform _transform { get; private set; }
    public Canvas _attackPoint { get; private set; }

    public void SetPosition(Transform transform)
    {
        _transform = transform;
    }

    public void SetCanvas(Canvas canvas)
    {
        _attackPoint = canvas;
    }

}
