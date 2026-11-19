using UnityEngine;
using UnityEngine.UI;

// FocusCanvas 컨트롤러
public class UIController : MonoBehaviour, IStateObserver
{
    [SerializeField] private Image _focusImage;
    [SerializeField] private RippleOnce rippleEffect;

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
        _focusImage.gameObject.SetActive(false);
        rippleEffect?.HideRipple();
    }
    public void FocusState()
    {
        _focusImage.gameObject.SetActive(true);
        rippleEffect?.PlayRipple();
    }
    public void PunishState()
    {
        _focusImage.gameObject.SetActive(false);
        rippleEffect?.HideRipple();
    }
    public void AttackState()
    {
        _focusImage.gameObject.SetActive(false);
        rippleEffect?.HideRipple();
    }
    public void DeadState()
    {
        _focusImage.gameObject.SetActive(false);
    }
    public void ClearState()
    {
        _focusImage.gameObject.SetActive(false);
    }
}
