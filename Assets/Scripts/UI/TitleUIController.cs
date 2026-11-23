using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _leftCanvas1;
    [SerializeField] private CanvasGroup _leftCanvas2;
    [SerializeField] private CanvasGroup _RightCanvas1;
    [SerializeField] private CanvasGroup _RightCanvas2;

    [SerializeField] private Image _sliceImgae;
    [SerializeField] private Image _sliceImgae2;

    [SerializeField] private RectTransform _rect;
    [SerializeField] private Vector2 _starPos;
    [SerializeField] private Quaternion _startRot;

    [SerializeField] private RectTransform _rect2;
    [SerializeField] private Vector2 _starPos2;
    [SerializeField] private Quaternion _startRot2;

    [SerializeField] private List<UIObjList> _sliceuiList = new();
   
    [SerializeField] private CanvasGroup _scrollCanvasGroup;
    [SerializeField] private Image _eyeFire;

    private Sequence _eyeFireSequence;

    private bool _isTransitioning;

    //private RectTransform _imageRect;
    //private Vector2 _starImagePos;
    //private Quaternion _startImageRot;

    //private RectTransform _imageRect2;
    //private Vector2 _starImagePos2;
    //private Quaternion _startImageRot2;

    

    private void Awake()
    {
        _rect = _leftCanvas1.GetComponent<RectTransform>();
        _rect2 = _RightCanvas1.GetComponent<RectTransform>();
        _eyeFire.gameObject.SetActive(false);
        //_imageRect = _sliceImgae.GetComponent<RectTransform>();
    }

    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        _sliceImgae.fillAmount = 0;
        _sliceImgae2.fillAmount = 0;

        UIObjList firstUI = new UIObjList
        {
            _imageRect = _sliceImgae.GetComponent<RectTransform>(),
            _startImagePos = _sliceImgae.GetComponent<RectTransform>().anchoredPosition,
            _startImageRot = _sliceImgae.GetComponent<RectTransform>().localRotation
        };

        _sliceuiList.Add(firstUI);

        UIObjList secondUI = new UIObjList
        {
            _imageRect = _sliceImgae2.GetComponent<RectTransform>(),
            _startImagePos = _sliceImgae2.GetComponent<RectTransform>().anchoredPosition,
            _startImageRot = _sliceImgae2.GetComponent<RectTransform>().localRotation
        };

        _sliceuiList.Add(secondUI);

        _starPos = _rect.anchoredPosition;
        _startRot = _rect.localRotation;
        
        _starPos2 = _rect2.anchoredPosition;
        _startRot2 = _rect2.localRotation;

        SetCanvasInput(_leftCanvas1, true);
        SetCanvasInput(_leftCanvas2, false);
        SetCanvasInput(_RightCanvas1, false);
        SetCanvasInput(_RightCanvas2, false);

        _scrollCanvasGroup.blocksRaycasts = false;
    }



    private void EyeFireOn()
    {
        _eyeFireSequence?.Kill();
        _eyeFire.DOKill();

        _eyeFire.gameObject.SetActive(true);

        _eyeFire.type = Image.Type.Filled;
        _eyeFire.fillMethod = Image.FillMethod.Horizontal;

        _eyeFire.fillOrigin = (int)Image.OriginHorizontal.Left;
        _eyeFire.fillAmount = 0f;

        Color color = _eyeFire.color;
        color.a = 1f;
        _eyeFire.color = color;

        _eyeFireSequence = DOTween.Sequence().Append(_eyeFire.DOFillAmount(1f, 0.2f).SetEase(Ease.OutExpo))
            .AppendCallback(() =>
            {
                _eyeFire.fillOrigin = (int)Image.OriginHorizontal.Right;
            })
            .Append(_eyeFire.DOFillAmount(0f, 0.25f).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                _eyeFire.gameObject.SetActive(false);
            });
    }

    public void StartSliceCanvas1()
    {
        if (_isTransitioning)
        {
            return;
        }

        EyeFireOn();

        _isTransitioning = true;
        SetCanvasInput(_leftCanvas1, false);

        StartCoroutine(SliceCanvas1());
    }

    public void StartSliceCanvas2()
    {
        if (_isTransitioning)
        {
            return;
        }
        EyeFireOn();

        _isTransitioning = true;
        SetCanvasInput(_leftCanvas2, false);

        StartCoroutine(SliceCanvas2());
    }

    public void StartSliceCanvasHorizontal1()
    {
        if (_isTransitioning)
        {
            return;
        }

        EyeFireOn();

        _isTransitioning = true;
        SetCanvasInput(_RightCanvas1, false);
        SetCanvasInput(_leftCanvas2, false);

        StartCoroutine(SliceCanvasHorizontal1());        
    }

    public void StartSliceCanvasHorizontal2()
    {
        if (_isTransitioning)
        {
            return;
        }

        _isTransitioning = true;        
        SetCanvasInput(_RightCanvas2, false);
        _scrollCanvasGroup.blocksRaycasts = false;

        StartCoroutine(SliceCanvasHorizontal2());
    }

    private IEnumerator Slice()
    {
        _sliceImgae.DOFillAmount(1f,0.1f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(0.03f);
        Vector2 direction = _sliceImgae.rectTransform.localRotation * Vector3.down;
        _sliceImgae.rectTransform.DOAnchorPos(_sliceImgae.rectTransform.anchoredPosition + direction * 1500f, 0.3f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(0.2f);
        _sliceImgae.DOFade(0f, 0.1f);
    }

    private void SetCanvasInput(CanvasGroup canvas, bool canInput)
    {
        canvas.interactable = canInput;
        canvas.blocksRaycasts = canInput;
    }

    private void ResetCanvas(RectTransform canvas)
    {
        canvas.transform.parent.GetComponentInParent<Canvas>().sortingOrder = 0;
        canvas.GetComponent<CanvasGroup>().alpha = 1f;
        canvas.anchoredPosition = _starPos;
        canvas.localRotation = _startRot;
    }

    private void ResetCanvas2(RectTransform canvas)
    {
        canvas.transform.parent.GetComponentInParent<Canvas>().sortingOrder = -2;
        canvas.GetComponent<CanvasGroup>().alpha = 1f;
        canvas.anchoredPosition = _starPos2;
        canvas.localRotation = _startRot2;
    }

    private void ResetSlice(RectTransform image, int num)
    {
        image.DOKill();
        image.anchoredPosition = _sliceuiList[num]._startImagePos;
        image.localRotation = _sliceuiList[num]._startImageRot;
        image.GetComponent<Image>().fillAmount = 0;
        Color imageColor = image.GetComponent<Image>().color;
        imageColor.a = 1f;
        image.GetComponent<Image>().color = imageColor;
    }

    private IEnumerator SliceCanvas1()
    {
        RectTransform rect = _leftCanvas1.GetComponent<RectTransform>();

        StartCoroutine(Slice());
        yield return new WaitForSeconds(0.3f);
        rect.DOLocalRotate(new Vector3(0, 0, 5), 0.1f).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.4f);
        rect.DOLocalRotate(new Vector3(0, 0, 15), 0.3f).SetEase(Ease.OutQuart);
        rect.DOAnchorPos(new Vector2(-250f, -200), 0.6f).SetEase(Ease.OutQuart);
        yield return _leftCanvas1.DOFade(0f, 0.6f).WaitForCompletion();
         
        _leftCanvas2.transform.parent.GetComponentInParent<Canvas>().sortingOrder = 1;
        ResetCanvas(rect);
        ResetSlice(_sliceImgae.GetComponent<RectTransform>(), 0);

        SetCanvasInput(_leftCanvas1, false);
        SetCanvasInput(_leftCanvas2, true);

        _isTransitioning = false;
    }

    private IEnumerator SliceCanvas2()
    {
        RectTransform rect2 = _leftCanvas2.GetComponent<RectTransform>();

        StartCoroutine(Slice());        
        yield return new WaitForSeconds(0.3f);
        rect2.DOLocalRotate(new Vector3(0, 0, 5), 0.1f).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.4f);
        rect2.DOLocalRotate(new Vector3(0, 0, 15), 0.3f).SetEase(Ease.OutQuart);
        rect2.DOAnchorPos(new Vector2(-250f, -200), 0.6f).SetEase(Ease.OutQuart);
        yield return _leftCanvas2.DOFade(0f, 0.6f).WaitForCompletion();

        _leftCanvas1.transform.parent.GetComponentInParent<Canvas>().sortingOrder = 1;
        ResetCanvas(rect2);
        ResetSlice(_sliceImgae.GetComponent<RectTransform>(), 0);

        SetCanvasInput(_leftCanvas2, false);
        SetCanvasInput(_leftCanvas1, true);

        _isTransitioning = false;
    }

    private IEnumerator Slice2()
    {
        _sliceImgae2.DOFillAmount(1f, 0.1f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(0.03f);
        _sliceImgae2.rectTransform.DOAnchorPos(new Vector2(1500f, _sliceImgae2.rectTransform.anchoredPosition.y), 0.3f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(0.2f);
        _sliceImgae2.DOFade(0f, 0.1f);
    }

    private IEnumerator SliceCanvasHorizontal1()
    {
        RectTransform rect = _RightCanvas1.GetComponent<RectTransform>();

        StartCoroutine(Slice2());
        yield return new WaitForSeconds(0.5f);
        //rect.DOLocalRotate(new Vector3(0, 0, 5), 0.1f).SetEase(Ease.InOutCubic);
        //yield return new WaitForSeconds(0.4f);
        //rect.DOLocalRotate(new Vector3(0, 0, 15), 0.3f).SetEase(Ease.OutQuart);
        rect.DOAnchorPos(new Vector2(250f, -200f), 0.6f).SetEase(Ease.OutQuart);
        yield return _RightCanvas1.DOFade(0f, 0.6f).WaitForCompletion();

        _RightCanvas2.transform.parent.GetComponentInParent<Canvas>().sortingOrder = -1;
        ResetCanvas2(rect);
        ResetSlice(_sliceImgae2.GetComponent<RectTransform>(), 1);

        SetCanvasInput(_leftCanvas2, false);
        SetCanvasInput(_RightCanvas1, false);
        SetCanvasInput(_RightCanvas2, true);

        _isTransitioning = false;
        _scrollCanvasGroup.blocksRaycasts = true;
    }

    private IEnumerator SliceCanvasHorizontal2()
    {
        RectTransform rect = _RightCanvas2.GetComponent<RectTransform>();

        StartCoroutine(Slice2());
        yield return new WaitForSeconds(0.5f);
        //rect.DOLocalRotate(new Vector3(0, 0, 5), 0.1f).SetEase(Ease.InOutCubic);
        //yield return new WaitForSeconds(0.4f);
        //rect.DOLocalRotate(new Vector3(0, 0, 15), 0.3f).SetEase(Ease.OutQuart);
        rect.DOAnchorPos(new Vector2(250f, -200f), 0.6f).SetEase(Ease.OutQuart);
        yield return _RightCanvas2.DOFade(0f, 0.6f).WaitForCompletion();

        _RightCanvas1.transform.parent.GetComponentInParent<Canvas>().sortingOrder = -1;
        ResetCanvas2(rect);
        ResetSlice(_sliceImgae2.GetComponent<RectTransform>(), 1);

        SetCanvasInput(_RightCanvas2, false);
        SetCanvasInput(_RightCanvas1, true);
        SetCanvasInput(_leftCanvas2, true);

        _isTransitioning = false;
    }
    public void LoadTutorial()
    {
        GameManager.Instance.TutorialSceneLoad();
    }

    public void LoadStage()
    {
        GameManager.Instance.StageSceneLoad();
    }

    public void LoadBoss()
    {
        GameManager.Instance.BossSceneLoad();
    }

    public void ExitButton()
    {
        GameManager.Instance.QuitGame();
    }
}
