using UnityEngine;

namespace SA
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(PlayerStateMachine player) : base(player) { }

        public override void EnterState()
        {
            // ��� ���¿� ������ �� ����Ǵ� �ڵ�
        }

        public override void ExitState()
        {
            // ��� ���¿��� ���� �� ����Ǵ� �ڵ�
        }

        public override void UpdateState()
        {
            // ��� ���¿��� ����Ǵ� �ڵ�
        }
    }
}