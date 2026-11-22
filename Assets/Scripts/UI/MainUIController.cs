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
    [SerializeField] private Button _nextButton;

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
        _gameOverButton.onClick.AddListener(GameManager.Instance.RetrySceneLoad);
        _gameOver.gameObject.SetActive(false);

        _nextButton.onClick.AddListener(GameManager.Instance.NextSceneSceneLoad); 
        _exitButton.onClick.AddListener(GameManager.Instance.TitleSceneSceneLoad);
        _gameClear.gameObject.SetActive(false);
    }

    public void IdleState()
    {        
        _gameOver.gameObject.SetActive(false);
        _gameClear.gameObject.SetActive(false);
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
        StartCoroutine(GameClear());
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        _gameOver.gameObject.SetActive(true);
        _gameOverButton.gameObject.SetActive(true);
        
    }

    private IEnumerator GameClear()
    {
        yield return new WaitForSeconds(1f);
        _gameClear.gameObject.SetActive(true);
    }
}
