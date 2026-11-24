using UnityEngine;

// 튜토리얼 적마다 부착
public class TutorialEnemyTarget : MonoBehaviour
{
    [SerializeField] private TutorialEnemyType _type;
    private bool _reported;

    public void ReportDefeated()
    {
        if (_reported) return;
        _reported = true;
        TutorialManager.Instance?.OnEnemyDefeated(_type);
    }
}
