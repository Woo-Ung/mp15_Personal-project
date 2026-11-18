using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SceneReference _titleScene;
    [SerializeField] private SceneReference _tutorialScene;
    [SerializeField] private SceneReference _stageScene;
    [SerializeField] private SceneReference _bossScene;       

    private void Awake()
    {
        SetSingletion();
    }

    public void TitleSceneSceneLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_titleScene.SceneName);
    }

    public void TutorialSceneLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_tutorialScene.SceneName);
    }

    public void StageSceneLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_stageScene.SceneName);
    }

    public void BossSceneLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_bossScene.SceneName);
    }

    public void RetrySceneLoad()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void SetSingletion()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
