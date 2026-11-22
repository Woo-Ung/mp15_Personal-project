using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ButtonDestory : MonoBehaviour
{
    [SerializeField] private Image _normalImage;
    [SerializeField] private Image _burnImage;
    [SerializeField] private Material _effectMaterial;
    [SerializeField] private ScrollButtonManager _scrollButtonManager;

    private Button _button;
    private Material _runtimeMaterial;
    private Tween _effectTween;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _runtimeMaterial = new Material(_effectMaterial);
        _burnImage.material = _runtimeMaterial;

        _runtimeMaterial.SetFloat("_FadeAmount", 0f);

        SetAlpha(_normalImage, 1f);
        SetAlpha(_burnImage, 0f);
    }

    public void PlayEffect()
    {
        if (_button != null)
            _button.interactable = false;

        _effectTween?.Kill();

        // 일반 이미지는 숨기고 불타는 이미지 표시
        SetAlpha(_normalImage, 0f);
        SetAlpha(_burnImage, 1f);

        _runtimeMaterial.SetFloat("_FadeAmount", 0f);

        _effectTween = _runtimeMaterial
            .DOFloat(1f, "_FadeAmount", 0.8f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                _scrollButtonManager.PlayDestroyButton();
            });
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void OnDestroy()
    {
        _effectTween?.Kill();

        if (_runtimeMaterial != null)
            Destroy(_runtimeMaterial);
    }
}