using UnityEngine;
using UnityEngine.EventSystems;

namespace SA
{
    public class PlayerMoveState : PlayerState
    {
        public PlayerMoveState(PlayerStateMachine player) : base(player) { }

        public override void EnterState()
        {

        }

        public override void ExitState()
        {
            player.currentSpeed = 0;
            player.anim.SetFloat("x", 0);
            player.anim.SetFloat("y", 0);
        }

        public override void UpdateState()
        {
            player.MoveCharacter();
        }

    }
}