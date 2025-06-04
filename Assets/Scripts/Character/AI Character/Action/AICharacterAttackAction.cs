using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "A.I/Actions/Attack")]
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("Attack")]
        [SerializeField] private string attackAnimation;

        [Header("Combo Action")]
        public AICharacterAttackAction comboAction;

        [Header("Action Values")]
        [SerializeField] AttackType attackType;
        public int attackWeight = 50;
        public float actionRecoveryTime = 1.5f;
        public float minimunAttackAngle = -35;
        public float maximumAttackAngle = 35;
        public float minimumAttackDistance = 0f;
        public float maximumAttackDistance = 2f;

        public void AttemptToPerformAction(AICharacterManager aiCharacter)
        {
            aiCharacter.characterAnimationManager.PlayTargetAttackAnimation(attackType, attackAnimation, true);
        }
    }
}