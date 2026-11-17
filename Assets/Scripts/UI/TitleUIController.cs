using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _leftCanvas1;
    [SerializeField] private CanvasGroup _leftCanvas2;
    [SerializeField] private Image _sliceImgae;

    [SerializeField] private RectTransform _rect;
    [SerializeField] private Vector2 _starPos;
    [SerializeField] private Quaternion _startRot;

    private RectTransform _imageRect;
    private Vector2 _starImagePos;
    private Quaternion _startImageRot;

    private void Awake()
    {
        _rect = _leftCanvas1.GetComponent<RectTransform>();
        _imageRect = _sliceImgae.GetComponent<RectTransform>();
    }

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        _sliceImgae.fillAmount = 0;
        
        _starPos = _rect.anchoredPosition;
        _startRot = _rect.localRotation;
       
        _starImagePos = _imageRect.anchoredPosition;
        _startImageRot = _imageRect.localRotation;
    }

    public void StartSliceCanvas1()
    {
        StartCoroutine(SliceCanvas1());
    }

    public void StartSliceCanvas2()
    {
        StartCoroutine(SliceCanvas2());
    }

    private void Slice()
    {
        _sliceImgae.DOFillAmount(1f,0.1f).SetEase(Ease.OutQuart);
    }

    private void ResetCanvas(RectTransform canvas)
    {
        canvas.transform.parent.GetComponentInParent<Canvas>().sortingOrder = -2;
        canvas.GetComponent<CanvasGroup>().alpha = 1f;
        canvas.anchoredPosition = _starPos;
        canvas.localRotation = _startRot;        
    }

    private void ResetSlice(RectTransform image)
    {
        image.DOKill();
        image.anchoredPosition = _starImagePos;
        image.localRotation = _startImageRot;
        image.GetComponent<Image>().fillAmount = 0;
        Color imageColor = image.GetComponent<Image>().color;
        imageColor.a = 1f;
        image.GetComponent<Image>().color = imageColor;
    }

    private IEnumerator SliceCanvas1()
    {
        RectTransform rect = _leftCanvas1.GetComponent<RectTransform>();

        Slice();
        yield return new WaitForSeconds(0.03f);
        _sliceImgae.rectTransform.DOAnchorPos(new Vector2(0, -1500f), 0.3f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(0.2f);
        _sliceImgae.DOFade(0f, 0.1f);
        yield return new WaitForSeconds(0.3f);
        rect.DOLocalRotate(new Vector3(0, 0, 5), 0.1f).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.4f);
        rect.DOLocalRotate(new Vector3(0, 0, 15), 0.3f).SetEase(Ease.OutQuart);
        rect.DOAnchorPos(new Vector2(-250f, -200), 0.6f).SetEase(Ease.OutQuart);
        yield return _leftCanvas1.DOFade(0f, 0.6f).WaitForCompletion();
         
        _leftCanvas2.transform.parent.GetComponentInParent<Canvas>().sortingOrder = -1;
        ResetCanvas(rect);
        ResetSlice(_sliceImgae.GetComponent<RectTransform>());
    }

    private IEnumerator SliceCanvas2()
    {
        RectTransform rect2 = _leftCanvas2.GetComponent<RectTransform>();

        Slice();
        yield return new WaitForSeconds(0.03f);
        _sliceImgae.rectTransform.DOAnchorPos(new Vector2(0, -1500f), 0.3f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(0.2f);
        _sliceImgae.DOFade(0f, 0.1f);
        yield return new WaitForSeconds(0.3f);
        rect2.DOLocalRotate(new Vector3(0, 0, 5), 0.1f).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.4f);
        rect2.DOLocalRotate(new Vector3(0, 0, 15), 0.3f).SetEase(Ease.OutQuart);
        rect2.DOAnchorPos(new Vector2(-250f, -200), 0.6f).SetEase(Ease.OutQuart);
        yield return _leftCanvas2.DOFade(0f, 0.6f).WaitForCompletion();

        _leftCanvas1.transform.parent.GetComponentInParent<Canvas>().sortingOrder = -1;
        ResetCanvas(rect2);
        ResetSlice(_sliceImgae.GetComponent<RectTransform>());
    }
}
