using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private SceneReference _bossScene;       

    private void Awake()
    {
        SetSingletion();
    }

    public void BossSceneLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_bossScene.SceneName);
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
