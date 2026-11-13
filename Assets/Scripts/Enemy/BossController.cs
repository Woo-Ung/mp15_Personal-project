using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossController : EnemyController
{
    [SerializeField] private int _life;
    [SerializeField] private float _dropSwordTime;
    [SerializeField] private PlayerAction _player;
    [SerializeField] private float _swordSpawnHeight;
    [SerializeField] private LayerMask _playerLayer;

    private BossEquipmenet _equipment => GetComponent<BossEquipmenet>();
    private SwordArray _currentArray;        
    private bool _canSummonSwordArray;
    private bool _canSummonSword;
    public bool _isClearArray { get; private set; }

    private SwordObj _punishSword;

    [SerializeField] private bool _isInNoticeRange;


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
    
    public void SetPunishSword(SwordObj sword)
    {
        _punishSword = sword;
    }

    protected override void PunishAttack()
    {   
         Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
         Vector3 target = _player.transform.position + Vector3.up;
         Vector3 direction = (target - origin).normalized;

         transform.position = _player.transform.position + (direction * 2f);
         transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
         _punishSword?.gameObject.SetActive(false);
         StageManager.Instance.StateUpdate(GameState.사망);        
    }

    public void PlayPunishAttack()
    {
        PunishAttack();
    }

    public override void ShootNoticeRay()
    {
        SetInRange(true);
    }
    public override void ShootAttackRay()
    {

    }
    public override void SetInRange(bool inR)
    {
        _isInNoticeRange = inR;
    }

    private IEnumerator SpawnSword()  // 통상패턴 1
    {
        if(!_canSummonSword)
        {
            yield break;
        }

        _canSummonSword = false;

        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
        Vector3 target = _player.transform.position + Vector3.up;
        Vector3 direction = (target - origin).normalized;
                
        GameObject sword = ObjectPool.Instance.Take();
        sword.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        sword.transform.position = target + (direction * 2f) + (Vector3.up * _swordSpawnHeight);
        
        yield return StartCoroutine(DropSword(sword.GetComponent<SwordObj>()));
        yield return StartCoroutine(DashAttack(sword));

        _canSummonSword = true;
    }

    private IEnumerator SpawnSwordArray() // 통상패턴 2
    {
        if (_canSummonSwordArray)
        {
            float currentTime1 = 0f;

            while (currentTime1 < 0.5f)
            {
                if (StageManager.Instance.PlayerState == GameState.기본)
                {
                    currentTime1 += Time.deltaTime;
                }
                yield return null;
            }

            _canSummonSwordArray = false;
            int num = UnityEngine.Random.Range(0, 3);
            _currentArray = _equipment._patternArray[num];
            _currentArray.gameObject.SetActive(true);
            _currentArray.SetActiveAll();
            _currentArray.transform.position = new Vector3(transform.position.x, transform.position.y + _swordSpawnHeight, transform.position.z);
            StartCoroutine(DropSword(_currentArray));
            StartCoroutine(CastingAttack());
        }
    }

    private IEnumerator CastingAttack() // 검진 생성 후 시전
    {
        float currentTime1 = 0f;

        while (currentTime1 < 8f && !_canSummonSwordArray)
        {            
            currentTime1 += Time.deltaTime;
           
            yield return null;
        }

        if(_canSummonSwordArray)// 검진 파훼
        {
            StageManager.Instance.StateUpdate(GameState.기본);
        }
        else // 검진 파훼 실패
        {
            _isClearArray = false;           
            StageManager.Instance.StateUpdate(GameState.응징);
        }
    }

    private IEnumerator GoUpArray(SwordArray currentArray)
    {
        Vector3 pos = currentArray.transform.position;

        float currentTime = 0f;

        while (currentTime < _dropSwordTime)
        {
            
            currentTime += Time.deltaTime;
            pos.y = Mathf.Lerp(pos.y, 200f, currentTime / _dropSwordTime);
            currentArray.transform.position = pos;
            
            yield return null;
        }
        pos.y = 200f;
        currentArray.transform.position = pos;

        _currentArray.gameObject.SetActive(false);
        _currentArray = null;

        SwordArray currentArray1 = _equipment._patternArray[4];
        currentArray1.gameObject.SetActive(true);
        currentArray1.SetActiveAll();
        currentArray1.transform.position = new Vector3(_player.transform.position.x, transform.position.y + _swordSpawnHeight, _player.transform.position.z);

        Vector3 pos1 = currentArray1.transform.position;

        float currentTime1 = 0f;

        while (currentTime1 < _dropSwordTime)
        {
            
            currentTime1 += Time.deltaTime;
            pos1.y = Mathf.Lerp(pos1.y, 0f, currentTime1 / _dropSwordTime);
            currentArray1.transform.position = pos1;

            yield return null;
        }

        pos1.y = 0f;
        currentArray1.transform.position = pos1;
                
        PunishAttack();
    }

    private IEnumerator DashAttack(GameObject sword)
    {
        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
        Vector3 target = sword.transform.position + Vector3.up;
        Vector3 direction = (target - origin).normalized;

        float currentTime1 = 0f;

        while (currentTime1 < 0.5f)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime1 += Time.deltaTime;
            }
            yield return null;
        }

        ScanRoute(target, out Vector3 hitPosition, out bool isPlayerExist);

        if (isPlayerExist)
        {
            ObjectPool.Instance.Return(sword);
            transform.position = sword.transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            StageManager.Instance.StateUpdate(GameState.사망);
        }
        else
        {
            ObjectPool.Instance.Return(sword);
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.position = sword.transform.position;

            StartCoroutine(SpawnSwordArray());
        }
    }

    private void ScanRoute(Vector3 nextPoint, out Vector3 hitPosition, out bool isPlayerExist, bool CheckRay = true) // 기본적으로 Ray, CheckRay = false 면 SphereCast발사
    {
        Vector3 me = new Vector3(transform.position.x, 1f, transform.position.z);
        Ray ray = new Ray(me, (nextPoint - me).normalized);
        Debug.DrawRay(me, (nextPoint - me), Color.blue, 5f);
        float distance = Vector3.Distance(me, nextPoint);

        if (CheckRay)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, distance, _playerLayer))
            {
                hitPosition = hit.point + hit.normal * -2f; // 플레이어 뒤 검이 떨어진 위치
                hitPosition = new Vector3(hitPosition.x, transform.position.y, hitPosition.z);
                isPlayerExist = true;
                Debug.Log("플레이어있음");
            }
            else
            {
                isPlayerExist = false;
                hitPosition = nextPoint; // 임시
            }
        }

        else
        {
            if (Physics.SphereCast(ray, 0.4f, out RaycastHit hit, distance, _playerLayer))
            {
                hitPosition = hit.point + hit.normal * -2f; // 플레이어 뒤 검이 떨어진 위치
                hitPosition = new Vector3(hitPosition.x, transform.position.y, hitPosition.z);
                isPlayerExist = true;
                Debug.Log("플레이어있음");
            }
            else
            {
                isPlayerExist = false;
                hitPosition = nextPoint; // 임시
            }
        }
    }       

    private IEnumerator DropSword(SwordArray currentArray) // 검 배열 드롭
    {
        float currentTime1 = 0f;

        while (currentTime1 < 0.5f)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime1 += Time.deltaTime;
            }
            yield return null;
        }

        Vector3 pos = currentArray.transform.position;

        float currentTime = 0f;

        while (currentTime < _dropSwordTime)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime += Time.deltaTime;
                pos.y = Mathf.Lerp(pos.y, 0f, currentTime / _dropSwordTime);
                currentArray.transform.position = pos;
            }
            yield return null;
        }

        pos.y = 0f;
        currentArray.transform.position = pos;

        while (currentTime1 < 0.9f)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime1 += Time.deltaTime;
            }
            yield return null;
        }
    }

    private IEnumerator DropSword(SwordObj currentArray) // 검 단일 드롭
    {
        float currentTime1 = 0f;

        while (currentTime1 < 0.5f)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime1 += Time.deltaTime;
            }
            yield return null;
        }

        Vector3 pos = currentArray.transform.position;

        float currentTime = 0f;

        while (currentTime < _dropSwordTime)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime += Time.deltaTime;
                pos.y = Mathf.Lerp(pos.y, 0f, currentTime / _dropSwordTime);
                currentArray.transform.position = pos;
            }
            yield return null;
        }

        pos.y = 0f;
        currentArray.transform.position = pos;
    }

  

    public override void IdleState()
    {
        if (!_isInNoticeRange)
        {
            if (_canSummonSwordArray)
            {
                StartCoroutine(SpawnSword());
            }
        }
        else
        {
            StartCoroutine(SpawnSwordArray());
        }
    }

    public void AllBroken()
    {
        _canSummonSwordArray = true;        
        _currentArray.gameObject.SetActive(false);
        _currentArray = null;
    }

    public override void FocusState()
    {

    }
    public override void PunishState()
    {
        if (!_isClearArray)
        {
            StartCoroutine(GoUpArray(_currentArray));
        }

        if (!_canSummonSwordArray)
        {
            for (int i = 0; i < _currentArray.Sword.Length; i++)
            {
                if (_currentArray.Sword[i].gameObject.activeInHierarchy)
                {
                    ScanRoute(_currentArray.Sword[i].transform.position, out Vector3 hitPosition, out bool isPlayerExist, false);

                    if(isPlayerExist)
                    {
                        SetPunishSword(_currentArray.Sword[i]);
                        PunishAttack();
                        break;
                    }
                    else 
                    {
                        continue;
                    }
                }
            }
        }
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
        _canSummonSwordArray = true;
        _canSummonSword = true;
        _isClearArray = true;
        _isInNoticeRange = false;
    }

    //protected override void OnDestroy()
    //{
    //    StageManager.Instance.Unsubscribe(this);
    //}
}
