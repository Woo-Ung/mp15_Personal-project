using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour, IStateObserver
{
    [SerializeField] private TextMeshProUGUI _gameOverImage;

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
        _gameOverImage.gameObject.SetActive(false);
    }
    public void FocusState()
    {
       
    }
    public void PunishState()
    {
       
    }
    public void AttackState()
    {
       
    }
    public void DeadState()
    {
        _gameOverImage.gameObject.SetActive(true);
    }
    public void ClearState()
    {
        
    }
}
