using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Image _focusImage;

    public static StageManager Instance { get; private set; }

    private void Awake()
    {
        SetSingleton();
    }
        

    public void OnFocusMode()
    {
        _focusImage.gameObject.SetActive(true);
    }

    public void OffFocusMode()
    {
        _focusImage.gameObject.SetActive(false);
    }


    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

}
