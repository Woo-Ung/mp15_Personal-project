using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour, IStateObserver
{
    [SerializeField] private Canvas _attackRouteCanvas;
    //[SerializeField ]private Image _directionImage;
    //[SerializeField] private Image _distanceImage;
    //[SerializeField] private GameObject _checkDummy;

    private GameState _charaState;

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
        //_directionImage.gameObject.SetActive(false);
        //_distanceImage.gameObject.SetActive(false);

        _attackRouteCanvas.gameObject.SetActive(false);

        _charaState = GameState.기본;
    }
    public void FocusState()
    {
        //_directionImage.gameObject.SetActive(true);
        //_distanceImage.gameObject.SetActive(true);
        //_distanceImage.fillAmount = 0;

        _attackRouteCanvas.gameObject.SetActive(true);

        _charaState = GameState.집중;      
        
    }
    
    public void PunishState()
    {

    }
    public void AttackState()
    {

    }
    public void DeadState()
    {

    }
    public void ClearState()
    {

    }

}
