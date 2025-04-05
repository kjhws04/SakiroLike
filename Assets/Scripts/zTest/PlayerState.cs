using SA;
using UnityEngine;

namespace SA
{
    public abstract class PlayerState
    {
        protected PlayerStateMachine player;

        public PlayerState(PlayerStateMachine player)
        {
            this.player = player;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
    }
}