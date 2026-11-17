
using UnityEngine;

[ExecuteAlways]
public class ToonPointLightSync : MonoBehaviour
{
    public Light sourceLight;
    public Renderer[] toonRenderers;

    [Range(0f, 2f)]
    public float strength = 0.35f;

    private MaterialPropertyBlock block;

    private void LateUpdate()
    {
        if (sourceLight == null) return;

        if (block == null)
            block = new MaterialPropertyBlock();

        foreach (Renderer target in toonRenderers)
        {
            if (target == null) continue;

            target.GetPropertyBlock(block);

            block.SetVector(
                "_ToonPointPos",
                sourceLight.transform.position
            );

            block.SetColor(
                "_ToonPointColor",
                sourceLight.color
            );

            block.SetFloat(
                "_ToonPointRange",
                Mathf.Max(0.01f, sourceLight.range)
            );

            block.SetFloat(
                "_ToonPointStrength",
                sourceLight.enabled ? strength : 0f
            );

            target.SetPropertyBlock(block);
        }
    }
}
