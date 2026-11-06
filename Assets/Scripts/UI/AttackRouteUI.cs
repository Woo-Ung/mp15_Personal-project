using UnityEngine;
using UnityEngine.UI;

public class AttackRouteUI : MonoBehaviour, IStateObserver
{
    [SerializeField] private Image _directionImage;
    [SerializeField] private Image _distanceImage;

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
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
