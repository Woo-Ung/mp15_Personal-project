using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollButtonManager : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //   public void OnBeginDrag(PointerEventData eventData)
    //{

    //}

    //public void OnDrag(PointerEventData eventData)
    //{

    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{

    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Scrollbar _scroll;
    [SerializeField] private GameObject _content;
    [SerializeField] private Button _easyButton;

    [SerializeField] private int _size;
    [SerializeField] private float _addValue;

    private void Awake()
    {
        Init();       
    }

    private void Start()
    {
        SetAddValue();
    }

    private void Init()
    {
        SetSize();
    }      

    private void SetSize()
    {
        _size = _content.transform.childCount;
    }
    private void SetAddValue()
    {
        _addValue = (1f / ((float)_size - 1));
    }

    public void RightButton()
    {
        _scroll.value += _addValue;
        _scroll.value = Mathf.Min(_scroll.value, 1f);
    }

    public void LeftButton()
    {
        _scroll.value -= _addValue;
        _scroll.value = Mathf.Max(_scroll.value, 0f);
    }

    public void PlayDestroyButton()
    {
        StartCoroutine(ButtonDestroy());
    }

    public IEnumerator ButtonDestroy()
    {
        _easyButton.gameObject.SetActive(false);
        Destroy(_easyButton.gameObject);
        _easyButton = null;

        yield return null;

        SetSize();
        SetAddValue();

        LayoutRebuilder.ForceRebuildLayoutImmediate(_content.GetComponent<RectTransform>());

        Canvas.ForceUpdateCanvases();

        LeftButton();
    }
}
