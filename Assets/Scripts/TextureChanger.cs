using DG.Tweening;
using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    [SerializeField] protected Material _baseMaterial;
    [SerializeField] protected Material _changeMaterial;

    protected MeshRenderer _meshRenderer;

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        _meshRenderer = GetComponent<MeshRenderer>();        
    }        

    public void ChangeMat()
    {
        if (_meshRenderer != null && _changeMaterial)
        {
            _meshRenderer.material = _changeMaterial;
            _meshRenderer.material.DOFade(0f, "_BaseColor", 0.1f);
            _meshRenderer.material.DOFade(1f,"_BaseColor", 2f);
        }
    }

    public void ChangeBaseMat()
    {
        if (_meshRenderer != null && _baseMaterial)
        {
            _meshRenderer.material = _baseMaterial;
        }
    }        
}