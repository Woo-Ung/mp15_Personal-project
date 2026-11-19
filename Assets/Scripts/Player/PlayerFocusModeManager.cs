using UnityEngine;
using UnityEngine.UI;

public class PlayerFocusModeManager : MonoBehaviour
{
    [SerializeField] private Image _timeImage;
    [SerializeField] private float _maxFocusTime;
    [field: SerializeField] public float _focusTime { get; private set; }
    [SerializeField] private float _chargingSpeed;
    [SerializeField] private float _usingSpeed;

    private void Update()
    {
        if(StageManager.Instance.PlayerState == GameState.기본)
        {
            ChargeFocusTime();
        }

        else if(StageManager.Instance.PlayerState == GameState.집중)
        {
            UsingFocusTime();
            if(_focusTime <= 0)
            {
                StageManager.Instance.StateUpdate(GameState.공격);
            }
        }
    }

    private void Start()
    {
        StageManager.Instance.PlayerSub(this);
    }

    private void ChargeFocusTime()
    {
        _focusTime += Time.deltaTime * _chargingSpeed;
        UpdateFocusTime();
    }

    private void UsingFocusTime()
    {
        _focusTime -= Time.deltaTime * _usingSpeed;
        UpdateFocusTime();
    }

    public void ChangeFocusTime(float value)
    {
        _focusTime += value;
        UpdateFocusTime();
    }

    private void UpdateFocusTime()
    {
        _focusTime = Mathf.Clamp(_focusTime, 0f, _maxFocusTime);
        _timeImage.fillAmount = _focusTime / _maxFocusTime;
    }       
}
