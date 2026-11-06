public interface IStateObserver
{
    public void IdleState(); // 기본 관찰 모드
    public void FocusState(); // 집중모드
    public void PunishState(); // 적 응징 시작
    public void AttackState(); // 플레이어 공격시작
    public void DeadState();
    public void ClearState(); 
}