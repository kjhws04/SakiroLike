using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "A.I/States/Attack")]
    public class AttackState : AIState
    {
        [HideInInspector] public AICharacterAttackAction currentAttack;
        [HideInInspector] public bool willPerformCombo = false;

        [Header("State Flags")]
        protected bool hasPerformedAttack = false; // 공격을 수행했는지 여부
        protected bool hasPerformedCombo = false; // 콤보를 수행했는지 여부

        [Header("Pivot After Attack")]
        [SerializeField] protected bool pivotAfterAttack = false;

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
            {
                return SwitchState(aiCharacter, aiCharacter.idle);
            }

            if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
            {
                return SwitchState(aiCharacter, aiCharacter.idle);
            }

            aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhilstAttacking(aiCharacter);

            aiCharacter.characterAnimationManager.UpdateanimatorMovementParameters(0, 0, false);

            if (willPerformCombo && !hasPerformedCombo)
            {
                if (currentAttack.comboAction != null)
                {
                    //
                    //hasPerformedCombo = true;
                    //currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
                }
            }

            if (aiCharacter.isPerformingAcion)
                return this;

            if (!hasPerformedAttack)
            {
                if (aiCharacter.aiCharacterCombatManager.actionRecoveryTime > 0)
                    return this;

                PerformAttack(aiCharacter);

                // 공격을 수행한 후, 다시 처음으로 돌아가서 콤보를 처리할 수 있는지 확인
                return this;
            }

            if (pivotAfterAttack)
                aiCharacter.aiCharacterCombatManager.PivotTowarsTarget(aiCharacter);

            return SwitchState(aiCharacter, aiCharacter.combat);
        }

        protected void PerformAttack(AICharacterManager aiCharacter)
        {
            hasPerformedAttack = true;

            if (currentAttack == null)
            {
                return;
            }

            currentAttack.AttemptToPerformAction(aiCharacter);
            aiCharacter.aiCharacterCombatManager.actionRecoveryTime = currentAttack.actionRecoveryTime;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasPerformedAttack = false;
            hasPerformedCombo = false;
        }
    }
}