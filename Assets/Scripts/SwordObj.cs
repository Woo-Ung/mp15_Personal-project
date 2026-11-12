using UnityEngine;

public class SwordObj : MonoBehaviour, IDamageable
{
    [field: SerializeField] public bool _canDestroy { get; private set; }

    [SerializeField] private TextureChanger _textureChanger;

    private const string LAYER_ENEMY = "Enemy";
    private const string LAYER_WALL = "Wall";

    private void Start()
    {
        Init();
    }

    private void Init()
    {        
        SetSwordType();
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
    }
}
