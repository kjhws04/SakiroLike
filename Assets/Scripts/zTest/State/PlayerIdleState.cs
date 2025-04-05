using UnityEngine;

namespace SA
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(PlayerStateMachine player) : base(player) { }

        public override void EnterState()
        {
            // 대기 상태에 진입할 때 실행되는 코드
        }

        public override void ExitState()
        {
            // 대기 상태에서 나갈 때 실행되는 코드
        }

        public override void UpdateState()
        {
            // 대기 상태에서 실행되는 코드
        }
    }
}