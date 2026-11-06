using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour, IStateObserver
{
    [SerializeField] private Canvas _attackRouteCanvas;
    //[SerializeField ]private Image _directionImage;
    //[SerializeField] private Image _distanceImage;
    //[SerializeField] private GameObject _checkDummy;

    private bool _isRotate;
    private bool _isDistance;

    private GameState _charaState;

    private void Start()
    {
        CacheComponents();
    }

    private void Update()
    {        
        if (_charaState == GameState.집중)
        {
            if (_isRotate == true)
            {
                SetAttackRoute();
            }
            if(_isDistance == true)
            {

            }
        }
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    public void IdleState()
    {
        //_directionImage.gameObject.SetActive(false);
        //_distanceImage.gameObject.SetActive(false);

        _attackRouteCanvas.gameObject.SetActive(false);

        _charaState = GameState.기본;
    }
    public void FocusState()
    {
        //_directionImage.gameObject.SetActive(true);
        //_distanceImage.gameObject.SetActive(true);
        //_distanceImage.fillAmount = 0;

        _attackRouteCanvas.gameObject.SetActive(true);

        _charaState = GameState.집중;
        _isRotate = true;
        _isDistance = false;
        
    }

    public void SetAttackRoute()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.up);

        if(plane.Raycast(ray,out float distance))
        {
            Vector3 direction = ray.GetPoint(distance) - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
    }

    public void PunishState()
    {

    }
    public void AttackState()
    {

    }
    public void DeadState()
    {

    }
    public void ClearState()
    {

    }

}
