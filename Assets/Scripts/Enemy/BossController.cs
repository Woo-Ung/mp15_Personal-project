using System;
using System.Collections;
using UnityEngine;

public class BossController : EnemyController
{
    [SerializeField] private int _life;
    [SerializeField] private float _dropSwordTime;

    private BossEquipmenet _equipment => GetComponent<BossEquipmenet>();

    private SwordArray _currentArray;
        
    private bool _canSummonSword;


    protected override void Start()
    {
        base.Start(); // 부모 클래스에 CacheComponents 등록됨
    }

    public override void Damaged()
    {
        _life -= 1;
        if(_life <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    protected override void PunishAttack()
    {

    }
    public override void ShootNoticeRay()
    {

    }
    public override void ShootAttackRay()
    {

    }
    public override void SetInRange(bool inR)
    {

    }

    private void SpwanSword()
    {

    }

    private void SpawnSwordArray()
    {
        if (_canSummonSword)
        {
            _canSummonSword = false;           
            int num = UnityEngine.Random.Range(0, 3);
            _currentArray = _equipment._patternArray[num];            
            _currentArray.gameObject.SetActive(true);
            _currentArray.SetActiveAll();
            _currentArray.transform.position = new Vector3(transform.position.x, transform.position.y + 6f, transform.position.z);
            StartCoroutine(DropSword(_currentArray));
        }
    }

    private IEnumerator DropSword(SwordArray currentArray)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 pos = _currentArray.transform.position;
        float currentTime = 0f;

        while (currentTime < _dropSwordTime)
        {
            currentTime += Time.deltaTime;
            pos.y = Mathf.Lerp(pos.y, 0f, currentTime / _dropSwordTime);
            _currentArray.transform.position = pos;

            yield return null;
        }

        pos.y = 0f;
        _currentArray.transform.position = pos;
    }

    public override void IdleState()
    {       
        SpawnSwordArray();
    }

    public void AllBroken()
    {
        _canSummonSword = true;
        _currentArray.gameObject.SetActive(false);
        _currentArray = null;
    }

    public override void FocusState()
    {

    }
    public override void PunishState()
    {

    }
    public override void AttackState()
    {

    }
    public override void DeadState()
    {

    }
    public override void ClearState()
    {

    }

    protected override void CacheComponents()
    {
        base.CacheComponents(); // 부모 클래스에서 옵저버 구독
        _canSummonSword = true;
    }

    //protected override void OnDestroy()
    //{
    //    StageManager.Instance.Unsubscribe(this);
    //}
}
