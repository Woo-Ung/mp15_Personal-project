using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwordObj : MonoBehaviour, IDamageable
{
    [field: SerializeField] public bool _canDestroy { get; private set; }

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private TextureChanger _textureChanger;
  
    [SerializeField] private Image _warningSign;

    private GameObject _warningCanvas;

    private const string WARNING_CANVAS = "EffectCanvas";
    private const string LAYER_ENEMY = "Enemy";
    private const string LAYER_WALL = "Wall";

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {       
        StartCoroutine(warningUI());
    }

    private void Init()
    {
        _warningCanvas = GameObject.Find(WARNING_CANVAS);
        SetSwordType();
    }

    private IEnumerator warningUI()
    {
        Physics.SyncTransforms();
        yield return null;
        Ray ray = new Ray(transform.position, -transform.up);
        //Debug.DrawRay(transform.position, -transform.up * 30f, Color.red, 5f); //확인용
        if(Physics.Raycast(ray, out RaycastHit hit, 30f, _groundLayer))
        {
            yield return null;            
            Image warning = Instantiate(_warningSign, hit.point + new Vector3(0, 0.01f, 0), _warningSign.transform.rotation);
            warning.transform.SetParent(_warningCanvas.transform);
            float currentTime = 0f;

            SwordArray array = GetComponentInParent<SwordArray>();

            while (currentTime < 0.6f)
            {
                if (array == null || array.BossControllerA == null)
                {
                    Destroy(warning.gameObject);
                    yield break;
                }

                if (StageManager.Instance.PlayerState == GameState.기본 ||
                    !array.BossControllerA._isClearArray)
                {
                    currentTime += Time.deltaTime;
                }

                yield return null;
            }

            Destroy(warning.gameObject);
        }
    }

    private void SetSwordType()
    {
        if(_canDestroy)
        {
            gameObject.layer = LayerMask.NameToLayer(LAYER_ENEMY);
            _textureChanger.ChangeMat();
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer(LAYER_WALL);
            _textureChanger.ChangeBaseMat();
        }
    }

    public void Damaged()
    {
        if (_canDestroy)
        {
            GetComponentInParent<SwordArray>()?.SwordDestroyed();
            gameObject.SetActive(false);
        }
        else
        {
            GetComponentInParent<SwordArray>()?.WrongSwordSAttacked(this);
        }
    }
}
