using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class AttackRouteUI : MonoBehaviour, IStateObserver
{
    [SerializeField] private Image _directionImage;
    [SerializeField] private Image _distanceImage;

    [SerializeField] private Image _testPointImage;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _maxDistance;

    private GameObject _ground;
    

    [SerializeField] private AttackRouteUI attackPoint;

    private bool _isRotate;

    private void Start()
    {
        CacheComponents();
    }

    private void Update()
    {     
        if (_isRotate == true)
        {
            SetAttackRoute();
        }
      
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
        _ground = GameObject.Find("Ground");
        Vector2 size = _distanceImage.rectTransform.sizeDelta;
        size.y = _maxDistance;
        _distanceImage.rectTransform.sizeDelta = size;
    }

    public void SetAttackRoute()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, _ground.transform.position);

        if (plane.Raycast(ray, out float distance))
        {            
            Vector3 direction = ray.GetPoint(distance) - transform.position;
            
            _distanceImage.fillAmount = Vector3.Distance(ray.GetPoint(distance), transform.position)/ _maxDistance;
            direction.y = 0f;

            float direction2 = direction.magnitude;

            if (direction2 > _maxDistance)
            {                
                _testPointImage.gameObject.transform.position = transform.position + direction.normalized * _maxDistance;
            }
            else
            {
                _testPointImage.gameObject.transform.position = ray.GetPoint(distance);
            }

            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }                     
        }
        if (Input.GetMouseButtonDown(0))
        {
            AttackRouteUI a = Instantiate(attackPoint, _testPointImage.gameObject.transform.position, attackPoint.transform.rotation);
            _testPointImage.gameObject.SetActive(false);
            _isRotate = false;
            a._isRotate = true;
        }
    }

    public void IdleState()
    {
        _directionImage.gameObject.SetActive(false);
        _distanceImage.gameObject.SetActive(false);
    }
    public void FocusState()
    {
        _directionImage.gameObject.SetActive(true);
        _distanceImage.gameObject.SetActive(true);
        _distanceImage.fillAmount = 0;
        _isRotate = true;      
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
