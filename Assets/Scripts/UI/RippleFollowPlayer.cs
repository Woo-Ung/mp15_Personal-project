
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RippleOnce : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image rippleImage;
    [SerializeField] private Transform player;
    [SerializeField] private Camera gameCamera;

    [Header("Animation")]
    [SerializeField] private float rippleDuration = 0.6f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Material rippleMaterial;
    private Coroutine rippleCoroutine;
    private Tween fadeTween;

    private static readonly int RippleCenter =
        Shader.PropertyToID("_RippleCenter");

    private static readonly int RippleRadius =
        Shader.PropertyToID("_RippleRadius");

    private static readonly int RippleProgress =
        Shader.PropertyToID("_RippleProgress");

    private static readonly int Aspect =
        Shader.PropertyToID("_Aspect");

    private void Awake()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;

        if (rippleImage == null || rippleImage.material == null)
        {
            Debug.LogError("Ripple Image와 Material을 연결해 주세요.", this);
            enabled = false;
            return;
        }

        // 다른 UI에 영향을 주지 않도록 머티리얼 복제
        rippleMaterial = new Material(rippleImage.material);
        rippleImage.material = rippleMaterial;

        // 시작할 때 파동 숨기기
        rippleMaterial.SetFloat(RippleProgress, -1f);
        SetAlpha(0f);
    }

    public void PlayRipple()
    {
        if (rippleMaterial == null ||
            rippleImage == null ||
            player == null ||
            gameCamera == null)
        {
            Debug.LogWarning("Ripple 참조가 연결되지 않았습니다.", this);
            return;
        }

        if (rippleCoroutine != null)
        {
            StopCoroutine(rippleCoroutine);
            rippleCoroutine = null;
        }

        fadeTween?.Kill();

        // 재생 시점의 플레이어 화면 위치
        Vector3 viewportPos =
            gameCamera.WorldToViewportPoint(player.position);

        if (viewportPos.z <= 0f)
            return;

        // UI Image는 화면 전체 Stretch로 설정
        Rect rect = rippleImage.rectTransform.rect;

        float aspect =
            rect.width / Mathf.Max(1f, rect.height);

        rippleMaterial.SetVector(
            RippleCenter,
            new Vector4(viewportPos.x, viewportPos.y, 0f, 0f)
        );

        rippleMaterial.SetFloat(Aspect, aspect);

        // 플레이어에서 화면의 가장 먼 모서리까지 거리
        float maxX = Mathf.Max(
            viewportPos.x, 1f - viewportPos.x
        ) * aspect;

        float maxY = Mathf.Max(
            viewportPos.y, 1f - viewportPos.y
        );

        float radius =
            Mathf.Sqrt(maxX * maxX + maxY * maxY);

        rippleMaterial.SetFloat(
            RippleRadius,
            radius + 0.15f
        );

        // 파동과 Fade 초기화
        rippleMaterial.SetFloat(RippleProgress, 0f);
        SetAlpha(0f);

        // DOTween Fade: 0 → 1 (0.5초)
        fadeTween = rippleImage
            .DOFade(1f, fadeDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

        // 파동은 한 번만 퍼짐
        rippleCoroutine =
            StartCoroutine(AnimateRipple());
    }

    private IEnumerator AnimateRipple()
    {
        float elapsed = 0f;

        while (elapsed < rippleDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, rippleDuration)
            );

            rippleMaterial.SetFloat(
                RippleProgress,
                t
            );

            yield return null;
        }

        // 파동 제거, Fade 결과는 유지
        rippleMaterial.SetFloat(RippleProgress, -1f);

        rippleCoroutine = null;
    }

    public void HideRipple()
    {
        if (rippleCoroutine != null)
        {
            StopCoroutine(rippleCoroutine);
            rippleCoroutine = null;
        }

        fadeTween?.Kill();

        if (rippleMaterial != null)
            rippleMaterial.SetFloat(RippleProgress, -1f);

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (rippleImage == null)
            return;

        Color color = rippleImage.color;
        color.a = alpha;
        rippleImage.color = color;
    }

    private void OnDestroy()
    {
        fadeTween?.Kill();

        if (rippleMaterial != null)
            Destroy(rippleMaterial);
    }
}
