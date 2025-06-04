using UnityEngine;

namespace SA
{
    public class AIState : ScriptableObject
    {
        public virtual AIState Tick(AICharacterManager aiCharacter)
        {
            return this;
        }

        protected virtual AIState SwitchState(AICharacterManager aiCharacterm, AIState newState)
        {
            ResetStateFlags(aiCharacterm);
            return newState;
        }

        protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
        {
            // Reset any state flags or variables here
        }
    }
}