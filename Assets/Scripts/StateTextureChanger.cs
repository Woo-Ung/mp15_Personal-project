using UnityEngine;

public class StateTextureChanger : TextureChanger, IStateObserver
{
    //[SerializeField] protected Material _baseMaterial; // 부모 클래스에서 정의
    //[SerializeField] protected Material _changeMaterial; // 부모 클래스에서 정의

    //protected MeshRenderer _meshRenderer; // 부모 클래스에서 정의

    //protected override void Awake()
    //{
    //    base.Awake();
    //}

    private void Start()
    {
        CacheComponents();
    }

    //protected override void Init()
    //{
    //    base.Init();        
    //}

    private void CacheComponents()
    {
        StageManager.Instance.Subscribe(this);
    }

    public void IdleState()
    {
        ChangeBaseMat();
    }   
    public void FocusState()
    {
        ChangeMat();
    }
    public void PunishState()
    {

    }
    public void AttackState()
    {
        ChangeBaseMat();
    }
    public void DeadState()
    {

    }
    public void ClearState()
    {

    }    

  
    private void OnDestroy()
    {
        StageManager.Instance.Unsubscribe(this);
    }
}