using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _swordPrefab;
    [SerializeField] private int _poolSize = 8;

    private GameObject[] _swords;
    private int _count;

    public static ObjectPool Instance { get; private set; }

    private void Awake()
    {
        SetSingleton();
    }

    private void Start()
    {
        FillPool();
    }

    private void FillPool()
    {
        _swords = new GameObject[_poolSize];

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject sword = Instantiate(_swordPrefab);
            sword.transform.SetParent(transform);
            sword.SetActive(false);

            _swords[i] = sword;
        }

        _count = _poolSize;

        Debug.Log($"SwordPool: {_count}개 생성");
    }

    public GameObject Take()
    {
        if(_count > 0)
        {
            _count--;

            GameObject sword = _swords[_count];
            sword.SetActive(true);

            return sword;
        }

        Debug.Log($"SwordPool: 남은 검 없음");
        return null;
    }

    public void Return(GameObject sword)
    {
        sword.SetActive(false);

        _swords[_count] = sword;
        _count++;
    }

    private void SetSingleton()
    {
        if(Instance != null && Instance !=  this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
