using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollSelectManager : MonoBehaviour
{
    [SerializeField] private Scrollbar _scroll;
    [SerializeField] private GameObject _content;
    [SerializeField] private int _size;
    [SerializeField] private float[] _pos;

    private float distance;
    private float _targetPos;

    private void Awake()
    {
        _pos = new float[_size];
    }

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        distance = 1f / ((float)_size - 1);
        for(int i = 0; i< _size; i++)
        {
            _pos[i] = distance * i;
        }
    }

    public void OnTab(int num)
    {
        _targetPos = _pos[num];
        DOTween.To(() => _scroll.value, x => _scroll.value = x, _targetPos, 0.3f);
    }  
  
}
