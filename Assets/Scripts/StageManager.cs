using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Image _focusImage;
    [SerializeField] private Camera _charaCamera;

    public static StageManager Instance { get; private set; }

    private void Awake()
    {
        SetSingleton();
        Init();
    }
        
    private void Init()
    {
        _focusImage.gameObject.SetActive(false);
        _charaCamera.gameObject.SetActive(false);
    }

    public void OnFocusMode()
    {
        _focusImage.gameObject.SetActive(true);
        _charaCamera.gameObject.SetActive(true);
    }

    public void OffFocusMode()
    {
        _focusImage.gameObject.SetActive(false);
        _charaCamera.gameObject.SetActive(false);
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
