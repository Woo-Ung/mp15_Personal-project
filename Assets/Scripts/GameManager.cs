using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public enum Level
{ 
    보통,
    어려움
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SceneReference _titleScene;
    [SerializeField] private SceneReference _tutorialScene;
    [SerializeField] private SceneReference _stageScene;
    [SerializeField] private SceneReference _bossScene;

    [field: SerializeField] public Level _level { get; private set; }

    private void Awake()
    {
        SetSingletion();
    }

    public void SetNormalLevel()
    {
        _level = Level.보통;
    }

    public void SetHardLevel()
    {
        _level = Level.어려움;
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

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void NextSceneSceneLoad()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == _tutorialScene.SceneName)
        {
            StageSceneLoad();
        }
        else if (currentScene.name == _stageScene.SceneName)
        {
            BossSceneLoad();
        }
    }

    public void RetrySceneLoad()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        StageManager.Instance.SetPause(true);

        FindFirstObjectByType<MainUIController>()?.SetPauseUI(true);
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        StageManager.Instance.SetPause(false);

        FindFirstObjectByType<MainUIController>()?.SetPauseUI(false);
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
