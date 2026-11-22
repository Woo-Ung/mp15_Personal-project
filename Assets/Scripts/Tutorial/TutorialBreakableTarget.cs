using UnityEngine;

// 파트 2의 파괴 가능 오브젝트에 부착
public class TutorialBreakableTarget : MonoBehaviour
{
    private bool _reported;

    public void ReportDestroyed()
    {
        if (_reported) return;
        _reported = true;
        TutorialManager.Instance?.OnObjectDestroyed();
    }
}
