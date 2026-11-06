using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    public Rotate _rotate { get; private set; }
    public float _distance  { get; private set; }
    public Position _position { get; private set; }

    public void SetRotate(Rotate rotate)
    {
        _rotate = rotate;
    }

    public void SetDistance(float distance)
    {
        _distance = distance;
    }

    public void SetPosition(Position position)
    {
        _position = position;
    }

}
