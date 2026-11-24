using UnityEngine;

public class Tutorial1 : MonoBehaviour, ITutorial
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private PlayerAction _player;

    public void StepStart()
    {
        _camera.transform.position = _player.transform.position;

    }

    public void StepMiddle()
    {

    }

    public void StepEnd()
    {

    }
}
