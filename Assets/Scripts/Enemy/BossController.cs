using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : EnemyController
{
    [SerializeField] private int _life;
    [SerializeField] private float _dropSwordTime;
    [SerializeField] private PlayerAction _player;
    [SerializeField] private float _swordSpawnHeight;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float FocusCoolTime;
    [SerializeField] private bool _isInNoticeRange;
    [SerializeField] private float _swordArrayTime;
    [SerializeField] private Collider _circleCollider;
    [SerializeField] private LineRendererPolygon _circleLine;

    [SerializeField] private GameObject _default;
    [SerializeField] private GameObject _arrayReady;
    [SerializeField] private GameObject _attack1;
    [SerializeField] private GameObject _attack2;
    [SerializeField] private GameObject _attack3;

    private BossEquipmenet _equipment => GetComponent<BossEquipmenet>();
    private SwordArray _currentArray;        
    private bool _canSummonSwordArray;
    private bool _canSummonSword;
    private bool _firstAttack;
    private Coroutine _focusTimer;
    private Coroutine _mainPattern;


    [field : SerializeField]public bool _isClearArray { get; private set; }

    private SwordObj _punishSword;

  


    protected override void Start()
    {
        base.Start(); // 부모 클래스에 CacheComponents 등록됨
        _default.SetActive(true);
        _attack1.SetActive(false);
        _attack2.SetActive(false);
        _attack3.SetActive(false);

        StageManager.Instance.StateUpdate(GameState.기본);
    }

    public override void Damaged()
    {
        _life -= 1;
        if(_life <= 0)
        {
            StageManager.Instance.StateUpdate(GameState.클리어);
            Destroy(gameObject);
        }
    }
    
    public void SetPunishSword(SwordObj sword)
    {
        _punishSword = sword;
    }

    protected override void PunishAttack()
    {        
        StageManager.Instance.StateUpdate(GameState.사망);
        StartCoroutine(PunishTerm());        
    }

    private IEnumerator PunishTerm()
    {
        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
        Vector3 target = _player.transform.position + Vector3.up;
        Vector3 direction = (target - origin).normalized;
        yield return new WaitForSeconds(0.5f);

        ChangeBossModel(1);
        transform.position = _player.transform.position + (direction * 2f);
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        _punishSword?.gameObject.SetActive(false);
    }

    public void PlayPunishAttack()
    {
        if (_mainPattern != null)
        {
            StopCoroutine(_mainPattern);
            _mainPattern = null;
        }

        PunishAttack();
    }

    public override void ShootNoticeRay()
    {
        SetInRange(true);
    }
    public override void ShootAttackRay()
    {
        StageManager.Instance.StateUpdate(GameState.사망);
    }
    public override void SetInRange(bool inR)
    {
        _isInNoticeRange = inR;
    }

    private IEnumerator PatternRoutine()
    {
        while (true)
        {           
            yield return StartCoroutine(SpawnSword());

            if (_firstAttack)
            {
                yield return StartCoroutine(SpawnSwordArray());
                _firstAttack = false;
            }
            else
            {
                int randInt0 = UnityEngine.Random.Range(0, 2);

                switch (randInt0)
                {
                    case 0:
                        yield return StartCoroutine(SpawnSwordArray());
                        break;
                    case 1:
                        yield return StartCoroutine(CircleAttack());
                        break;
                    default:
                        break;
                }
            }

            yield return new WaitForFixedUpdate();

            if (_isClearArray)
            {
                if (_isInNoticeRange)
                {
                    yield return StartCoroutine(CircleAttack());
                }
                else
                {
                    int randInt = UnityEngine.Random.Range(0, 2);

                    switch (randInt)
                    {
                        case 0:
                            continue;                            
                        case 1:
                            yield return StartCoroutine(DropSwordCombo());
                            break;
                        default:
                            continue;
                    }
                }
            }
            else
            {
                yield return StartCoroutine(GoUpArray(_currentArray));
            }

            if (_isInNoticeRange)
            {
                yield return StartCoroutine(CircleAttack());
            }
            else
            {
                int randInt1 = UnityEngine.Random.Range(0, 2);

                switch (randInt1)
                {
                    case 0:
                        continue;
                    case 1:
                        yield return StartCoroutine(DropSwordCombo());
                        break;
                    default:
                        continue;
                }
            }

            yield return null;
        }
    }

    private IEnumerator SpawnSword()  // 통상패턴 1
    {
        if(!_canSummonSword)
        {
            yield break;
        }
        ChangeBossModel(0);

        _canSummonSword = false;

        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
        Vector3 target = _player.transform.position + Vector3.up;
        Vector3 direction = (target - origin).normalized;
                
        GameObject sword = ObjectPool.Instance.Take();        

        sword.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        sword.transform.position = target + (direction * 2f) + (Vector3.up * _swordSpawnHeight);

        if (!sword.gameObject.activeInHierarchy)
        {
            _canSummonSword = true;
            yield break;
        }

        yield return StartCoroutine(DropSword(sword.GetComponent<SwordObj>()));

        if (!sword.gameObject.activeInHierarchy)
        {
            _canSummonSword = true;
            yield break;
        }

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

            
            int num = UnityEngine.Random.Range(0, 3);
            _currentArray = _equipment._patternArray[num];
            _currentArray.gameObject.SetActive(true);
            _currentArray.SetActiveAll();
            _canSummonSwordArray = false;
            _currentArray.transform.position = new Vector3(transform.position.x, transform.position.y + _swordSpawnHeight, transform.position.z);
            
            yield return StartCoroutine(DropSword(_currentArray));            
            ChangeBossModel(4);
            yield return StartCoroutine(CastingAttack());
        }
    }

    private IEnumerator DropSwordCombo() // 통상패턴3
    {       
        for (int i = 0; i < 3; i++)
        {
            ChangeBossModel(0);
            Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
            Vector3 target = _player.transform.position + Vector3.up;
            Vector3 direction = (target - origin).normalized;

            GameObject sword = ObjectPool.Instance.Take();

            if (sword == null)
            {
                yield break;
            }

            sword.GetComponent<SwordObj>().SetDestroy(true);
            sword.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            sword.transform.position = target + (Vector3.up * _swordSpawnHeight);
            ChangeBossModel(3);
            yield return StartCoroutine(DropSword(sword.GetComponent<SwordObj>()));
        }        
    }

    private IEnumerator CircleAttack() // 통상패턴4
    {
        ChangeBossModel(0);
        _circleLine.gameObject.SetActive(true);
        float currentTime = 0f;

        while (currentTime < 0.6f)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime += Time.deltaTime;
            }
            yield return null;
        }

        _circleCollider.gameObject.SetActive(true);
        ChangeBossModel(2);
        float currentTime1 = 0f;

        while (currentTime1 < 0.3f)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime1 += Time.deltaTime;
            }
            yield return null;
        }

        _circleLine.gameObject.SetActive(false);
        _circleCollider.gameObject.SetActive(false);
        ChangeBossModel(0);
    }

    private IEnumerator CastingAttack() // 검진 생성 후 시전
    {
        float currentTime1 = 0f;

        while (currentTime1 < _swordArrayTime && !_canSummonSwordArray)
        {
            if (StageManager.Instance.PlayerState == GameState.기본)
            {
                currentTime1 += Time.deltaTime;               
            }
            yield return null;
        }

        if(_canSummonSwordArray)// 검진 파훼
        {
            yield return new WaitUntil(() => StageManager.Instance.PlayerState != GameState.공격);
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
            if(StageManager.Instance.PlayerState != GameState.사망)
            { 
            currentTime += Time.deltaTime;
            pos.y = Mathf.Lerp(pos.y, 200f, currentTime / _dropSwordTime);
            currentArray.transform.position = pos;
            }
            
            yield return null;
        }
        pos.y = 200f;
        currentArray.transform.position = pos;

        _currentArray.gameObject.SetActive(false);
        _currentArray = null;

        ChangeBossModel(3);

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
                
        PlayPunishAttack();
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
        ChangeBossModel(1);
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

        ChangeBossModel(4);
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

        ChangeBossModel(3);
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

    private IEnumerator FocusTimer() // 집중상태 x초 이상 유지 시 즉사 패턴
    {
        float currentTime = 0;
        while (currentTime < FocusCoolTime)
        {            
            if(StageManager.Instance.PlayerState != GameState.집중)
            {               
                _focusTimer = null;
                yield break;
            }
            currentTime += Time.deltaTime;
            yield return null;
        }

        _focusTimer = null;
        if (StageManager.Instance.PlayerState == GameState.집중)
        {
            PlayPunishAttack();
        }
    }

    public override void IdleState()
    {
        if(_mainPattern != null)
        {
            return;
        }

        _mainPattern = StartCoroutine(PatternRoutine());
    }

    

    public override void FocusState()
    {
        if(_focusTimer != null)
        {
            StopCoroutine( _focusTimer );
            _focusTimer = null;
        }

        _focusTimer = StartCoroutine(FocusTimer());
    }
    public override void PunishState()
    {       
        if (!_canSummonSwordArray && _isClearArray)
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
            for (int i = 0; i < _equipment._swordPool.transform.childCount; i++)
            {
                if (_equipment._swordPool.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    ScanRoute(_equipment._swordPool.transform.GetChild(i).position, out Vector3 hitPosition, out bool isPlayerExist, false);

                    if (isPlayerExist)
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

    private void LookPlayer()
    {
        if (_player.transform != null)
        {
            transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
        }
    }
    public void AllBroken()
    {
        _canSummonSwordArray = true;
        _currentArray.gameObject.SetActive(false);
        _currentArray = null;
    }
    protected override void CacheComponents()
    {
        base.CacheComponents(); // 부모 클래스에서 옵저버 구독
        _canSummonSwordArray = true;
        _canSummonSword = true;
        _isClearArray = true;
        _isInNoticeRange = false;
        _circleCollider.gameObject.SetActive(false);
        _circleLine.gameObject.SetActive(false);
        _firstAttack = true;
    }
    private void ChangeBossModel(int pattern)
    {
        LookPlayer();

        _default.SetActive(pattern == 0);
        _attack1.SetActive(pattern == 1);
        _attack2.SetActive(pattern == 2);
        _attack3.SetActive(pattern == 3);
        _arrayReady.SetActive(pattern == 4);
    }


    //protected override void OnDestroy()
    //{
    //    StageManager.Instance.Unsubscribe(this);
    //}
}
