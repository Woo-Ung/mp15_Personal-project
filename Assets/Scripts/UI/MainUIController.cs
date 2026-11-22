using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour, IStateObserver
{
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private Button _gameOverButton;
    [SerializeField] private GameObject _gameClear;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
        _gameOverButton.onClick.AddListener(GameManager.Instance.RetrySceneLoad);
        _gameOverButton.gameObject.SetActive(false);
        _gameOver.gameObject.SetActive(false);
    }

    public void IdleState()
    {
        _gameOverButton.gameObject.SetActive(false);
        _gameOver.gameObject.SetActive(false);       
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
        _gameOver.gameObject.SetActive(true);
        _gameOverButton.gameObject.SetActive(true);
        
    }
}
