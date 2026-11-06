using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class AttackRouteUI : MonoBehaviour, IStateObserver
{
    [SerializeField] private Image _directionImage;
    [SerializeField] private Image _distanceImage;
    [SerializeField] private LayerMask _groundLayer;


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
    }

    public void SetAttackRoute()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {            
            Vector3 direction = ray.GetPoint(distance) - transform.position;
            _distanceImage.fillAmount = Vector3.Distance(ray.GetPoint(distance), transform.position)/ 15;
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));          
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                AttackRouteUI a = Instantiate(attackPoint, hit.point, attackPoint.transform.rotation);
                _isRotate = false;
                a._isRotate = true;
            }
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
