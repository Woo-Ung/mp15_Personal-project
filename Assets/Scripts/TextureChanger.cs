using UnityEngine;

public class TextureChanger : MonoBehaviour, IStateObserver
{
    [SerializeField] private Material _idleMaterial;
    [SerializeField] private Material _focusMaterial;

    private MeshRenderer _meshRenderer;
      
    private void Start()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        StageManager.Instance.Subscribe(this);
    }

    public void IdleState()
    {
        ChangeIdleMat();
    }   
    public void FocusState()
    {
        ChangeFocusMat();
    }
    public void PunishState()
    {

    }
    public void AttackState()
    {
        ChangeIdleMat();
    }
    public void DeadState()
    {

    }
    public void ClearState()
    {

    }
    

    private void ChangeFocusMat()
    {
        if (_meshRenderer != null && _focusMaterial)
        {
            _meshRenderer.material = _focusMaterial;
        }
    }

    private void ChangeIdleMat()
    {
        if (_meshRenderer != null && _idleMaterial)
        {
            _meshRenderer.material = _idleMaterial;
        }
    }
}