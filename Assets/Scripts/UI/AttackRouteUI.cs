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

    private PlayerAction _playerAction;
    private GameObject _ground;
    private bool _isSelectNow = false;
    

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
        _isRotate = true;
         _ground = GameObject.Find("Ground");
        Vector2 size = _distanceImage.rectTransform.sizeDelta;
        size.y = _maxDistance;
        _distanceImage.rectTransform.sizeDelta = size;
        _playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
        _isSelectNow = true;
        AttackPointSave();
    }

    public void AttackPointSave()
    {
        PlayerAttackPoint point = new();
        point.SetPosition(_testPointImage.GetComponentInParent<AttackRouteUI>().gameObject.transform);
        point.SetCanvas(gameObject.GetComponent<Canvas>());
        _playerAction.AddAttackPoint(point);
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
            AttackRouteUI a = Instantiate(attackPoint, _testPointImage.gameObject.transform.position + new Vector3(0, 0.1f, 0), attackPoint.transform.rotation);
            _testPointImage.gameObject.SetActive(false);
            _isRotate = false;
            _isSelectNow = false;
            a._isRotate = true;
        }
    }

    public void IdleState()
    {
        
    }
    public void FocusState()
    {       
        _distanceImage.fillAmount = 0;             
    }
    public void PunishState()
    {
        
    }
    public void AttackState()
    {
        if(_isSelectNow == true)
        {
            _distanceImage.gameObject.SetActive(false);
            _testPointImage.gameObject.SetActive(false);           
        }
        _isRotate = false;
        _directionImage.color = new Color(1f, 0.5f, 0.5f, 1f);
        _distanceImage.color = new Color(1f, 0.5f, 0.5f, 1f);
    }
    public void DeadState()
    {
       
    }
    public void ClearState()
    {
        
    }
}
