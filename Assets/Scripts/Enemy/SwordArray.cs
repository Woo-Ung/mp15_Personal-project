using UnityEngine;

public class SwordArray: MonoBehaviour
{
    
    [SerializeField] private SwordObj[] _sword;
    [SerializeField] private BossController _bossController;
    [field: SerializeField] public int _destroySwordCount { get; private set; }

    public int _arrayNum { get; private set; }

    private GameObject _swordArray => gameObject;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Debug.Log($"파괴가능검 수: {_destroySwordCount}");
    }

    private void OnEnable()
    {
        SetArray();
    }

    private void Init()
    {        
        _sword = new SwordObj[_swordArray.transform.childCount];
        SetArray();
    }

    public void SetActiveAll()
    {
        for (int i = 0; i < _arrayNum; i++)
        {
            _sword[i].gameObject.SetActive(true);
        }
    }

    public void SwordDestroyed()
    {
        _destroySwordCount--;
        if(_destroySwordCount <= 0)
        {
            _bossController.AllBroken();
        }
    }

    private void SetArray()
    {
        _destroySwordCount = 0;
        _arrayNum = _swordArray.transform.childCount;
        for (int i = 0; i < _arrayNum; i++)
        {
            _sword[i] = _swordArray.transform.GetChild(i).GetComponent<SwordObj>();
            if (_sword[i]._canDestroy)
            {
                _destroySwordCount++;
            }
        }
    }
}