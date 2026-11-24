
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RippleOnce : MonoBehaviour
{
    [SerializeField] private Image rippleImage;
    [SerializeField] private Transform player;
    [SerializeField] private Camera gameCamera;

    [SerializeField] private float rippleDuration = 0.6f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Material rippleMaterial;
    private Coroutine rippleCoroutine;
    private Tween fadeTween;

    private static readonly int RippleCenter = Shader.PropertyToID("_RippleCenter");

    private static readonly int RippleRadius = Shader.PropertyToID("_RippleRadius");

    private static readonly int RippleProgress = Shader.PropertyToID("_RippleProgress");

    private static readonly int Aspect = Shader.PropertyToID("_Aspect");

    private void Awake()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;

        if (rippleImage == null || rippleImage.material == null)
        {
            enabled = false;
            return;
        }

        rippleMaterial = new Material(rippleImage.material);
        rippleImage.material = rippleMaterial;

        
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
            return;
        }

        if (rippleCoroutine != null)
        {
            StopCoroutine(rippleCoroutine);
            rippleCoroutine = null;
        }

        fadeTween?.Kill();

       
        Vector3 viewportPos =
            gameCamera.WorldToViewportPoint(player.position);

        if (viewportPos.z <= 0f)
            return;

     
        Rect rect = rippleImage.rectTransform.rect;

        float aspect =
            rect.width / Mathf.Max(1f, rect.height);

        rippleMaterial.SetVector(
            RippleCenter,
            new Vector4(viewportPos.x, viewportPos.y, 0f, 0f)
        );

        rippleMaterial.SetFloat(Aspect, aspect);

        
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

        rippleMaterial.SetFloat(RippleProgress, 0f);
        SetAlpha(0f);

        fadeTween = rippleImage
            .DOFade(1f, fadeDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

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
