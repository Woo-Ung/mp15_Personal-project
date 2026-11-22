using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TutorialEnemyType
{
    First,
    Part2,
    Moving,
    Archer
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    // 씬을 다시 로드해도 유지되는 체크포인트. 다른 씬에서 튜토리얼을 새로 시작할 때 ResetProgress() 호출.
    private static int s_checkpoint;
    private static string s_checkpointScene;

    [SerializeField] private GameObject _camera;

    [Header("각 파트에 해당하는 적과 파괴 오브젝트를 묶은 부모")]
    [SerializeField] private GameObject[] _partRoots = new GameObject[3];

    [Header("파트 시작 시 플레이어가 이동할 위치")]
    [SerializeField] private Transform[] _spawnPoints = new Transform[3];
    [SerializeField] private Transform _player;

    [Header("UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _guidePanel;
    [SerializeField] private TMP_Text _guideText;

    [Header("파트 안내 문구")]
    [SerializeField] private GameObject[] _dialogues;
    [SerializeField] private GameObject[] _guides;

    private int _part;
    private bool _focusEntered;
    private bool _firstDead;
    private bool _objectDestroyed;
    private int _part2Kills;
    private bool _movingDead;
    private bool _archerDead;
    private bool _attackInProgress;
    private bool _waitingForNext;
    private bool _complete;

    public bool IsDialogueOpen => _waitingForNext;

    private void Awake()
    {
        Instance = this;
        string sceneName = SceneManager.GetActiveScene().name;
        if (s_checkpointScene != sceneName)
        {
            s_checkpoint = 0;
            s_checkpointScene = sceneName;
        }
    }

    private void Start()
    {
        BeginPart(Mathf.Clamp(s_checkpoint, 0, 2));
    }
    private void Update()
    {
        if (_waitingForNext &&
         (Input.GetKeyDown(KeyCode.Return) ||
          Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            NextDialogue();
        }

        // 기본 상태일 때 튜토리얼 진행 조건 검사
        if (!_waitingForNext &&
            !_complete &&
            StageManager.Instance.PlayerState == GameState.기본)
        {
            CheckCompletion();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public static void ResetProgress()
    {
        s_checkpoint = 0;
        s_checkpointScene = null;
    }

    private void BeginPart(int part)
    {
        _part = part;
        _focusEntered = false;
        _firstDead = false;
        _objectDestroyed = false;
        _part2Kills = 0;
        _movingDead = false;
        _archerDead = false;
        _attackInProgress = false;
        _waitingForNext = false;
        _complete = false;

        for (int i = 0; i < _partRoots.Length; i++)
            if (_partRoots[i] != null) _partRoots[i].SetActive(i == part);

        // 재시작 시 플레이어를 해당 파트 시작 지점으로 이동
        if (_player != null && part < _spawnPoints.Length && _spawnPoints[part] != null)
        {
            CharacterController controller = _player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            _player.SetPositionAndRotation(_spawnPoints[part].position, _spawnPoints[part].rotation);
            Physics.SyncTransforms();
            if (controller != null) controller.enabled = true;
            _camera.transform.position = _player.position;
        }

        ShowDialogue();
    }

    // 대사 다음 버튼에 연결

    public void NextDialogue()
    {
        if (!_waitingForNext) return;

        HideAllTutorialUI();

        if (_part >= 0 && _part < _guides.Length &&
            _guides[_part] != null)
        {
            _guides[_part].SetActive(true);
        }

        _waitingForNext = false;

        // 대사 종료 후 이미 충족된 조건도 확인
        CheckCompletion();
    }

    // PlayerAction: 집중 상태에 실제 진입할 때 호출
    public void OnFocusEntered()
    {
        if (_part == 0) _focusEntered = true;
    }

    // PlayerAction: 공격 실행 시작/종료 시 호출
    public void OnAttackStarted() => _attackInProgress = true;

    public void OnAttackFinished()
    {
        _attackInProgress = false;
        CheckCompletion();
    }

    // TutorialEnemyTarget.ReportDefeated()에서 호출
    public void OnEnemyDefeated(TutorialEnemyType type)
    {
        Debug.Log($"적 처치: {type} / 현재 파트: {_part}");

        if (_complete || _waitingForNext) return;

        if (_part == 0 && type == TutorialEnemyType.First)
            _firstDead = true;

        if (_part == 1 && type == TutorialEnemyType.Part2)
            _part2Kills++;

        if (_part == 2 && type == TutorialEnemyType.Moving)
            _movingDead = true;

        if (_part == 2 && type == TutorialEnemyType.Archer)
            _archerDead = true;

        CheckCompletion();
    }

    // TutorialBreakableTarget.ReportDestroyed()에서 호출
    public void OnObjectDestroyed()
    {
        if (_part != 1 || _complete || _waitingForNext) return;
        _objectDestroyed = true;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (_waitingForNext || _complete)
            return;

        if (StageManager.Instance.PlayerState != GameState.기본)
            return;

        bool success = _part switch
        {
            0 => _focusEntered && _firstDead,
            1 => _objectDestroyed && _part2Kills >= 2,
            2 => _movingDead && _archerDead,
            _ => false
        };

        if (!success) return;

        _guidePanel.SetActive(false);
        if (_part == 2)
        {
            _complete = true;
            s_checkpoint = 0;
            StageManager.Instance.StateUpdate(GameState.클리어);
            return;
        }

        _waitingForNext = true;
        s_checkpoint = _part + 1;
        BeginPart(_part + 1);
    }

    private void HideAllTutorialUI()
    {
        foreach (GameObject dialogue in _dialogues)
        {
            if (dialogue != null)
                dialogue.SetActive(false);
        }

        foreach (GameObject guide in _guides)
        {
            if (guide != null)
                guide.SetActive(false);
        }
    }

    private void ShowDialogue()
    {
        HideAllTutorialUI();

        if (_part >= 0 && _part < _dialogues.Length &&
            _dialogues[_part] != null)
        {
            _dialogues[_part].SetActive(true);
        }

        _waitingForNext = true;
    }

    // 사망 연출이 끝난 시점이나 재시작 버튼에서 호출
    public void RetryCurrentPart()
    {
        Time.timeScale = 1f;
        s_checkpoint = _part;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
