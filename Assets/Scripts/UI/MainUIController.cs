using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour, IStateObserver
{
    [SerializeField] private Button _gameOverButton;
    [SerializeField] private TextMeshProUGUI _gameOverImage;

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
        _gameOverButton.onClick.AddListener(GameManager.Instance.RetrySceneLoad);
    }

    public void IdleState()
    {
        _gameOverImage.gameObject.SetActive(false);
        _gameOverButton.gameObject.SetActive(false);
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
        StartCoroutine(GameOver());       
    }
    public void ClearState()
    {
        
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        _gameOverButton.gameObject.SetActive(true);
        _gameOverImage.gameObject.SetActive(true);
    }
}
