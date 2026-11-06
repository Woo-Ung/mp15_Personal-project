using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IStateObserver
{
    [SerializeField] private Image _focusImage;

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
    }
    public void FocusState()
    {
        _focusImage.gameObject.SetActive(true);
    }
    public void PunishState()
    {
        _focusImage.gameObject.SetActive(false);
    }
    public void AttackState()
    {
        _focusImage.gameObject.SetActive(false);
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
