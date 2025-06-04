using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace SA
{
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
    public class CombatState : AIState
    {
        // �� AI ĳ���ʹ� ������ ���� ����Ʈ�� ����
        [Header("Attacks")]
        public List<AICharacterAttackAction> aiCharacterAttacks; // ���� ��Ȳ���� �ĺ����� �Ǵ� ���� ������� �� ����
        protected List<AICharacterAttackAction> potentialAttacks; // ������ ����� �� �ִ� ���ݸ� ���͸� �� ���� (�þ߰�, �Ÿ�, ���� ���)
        private AICharacterAttackAction chooseAttack; // ���õ� ���� ���
        private AICharacterAttackAction previousAttack; // ������ ���õ� ���� ���
        protected bool hasAttack = false; // AI�� ������ �õ��ߴ��� ����

        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false; // �޺��� ������ �� �ִ��� ����
        [SerializeField] protected int chanceToPerformCombo = 25; // �޺��� �õ� Ȯ�� (0~100 ������ ��)
        protected bool hasRolledForComboChance = false; // �޺��� ������ Ȯ���� ����ߴ��� ����

        [Header("Engagement Distance")]
        [SerializeField] public float maxiumEngagementDistance = 5f; // AI�� ������ �õ��� �� �ִ� �ִ� �Ÿ�

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAcion)
                return this;

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // AI ĳ���Ͱ� �þ߰�(FOV) �ۿ� ���� �� Ÿ���� �ٶ�
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    aiCharacter.aiCharacterCombatManager.PivotTowarsTarget(aiCharacter);
            }

            // Ÿ���� ���� ȸ��
            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            // Ÿ���� ������ idle ���·� ��ȯ
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // ������ ���õ��� �ʾҴٸ�, �ٽ� ������ ����
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = chooseAttack;
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            // ���� ���� �������� �ָ� ������ ���� ����(pursueTarget)�� ��ȯ
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maxiumEngagementDistance)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            }

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();

            // 1. ������ ��� ���� ����� ��ȸ (���� ���ǿ� �������� ���� ������ ����)
            foreach (var potentialAttack in aiCharacterAttacks)
            {
                if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                if (potentialAttack.minimunAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                // ���� ���� ����� potentialAttack�� ����
                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0)
            {
                Debug.Log("No potential attacks available for the AI character.");
                return;
            }

            // 2. ����ġ ��� ���� ����
            var totalWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(0, totalWeight + 1);
            var processedWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight;

                if (randomWeightValue <= processedWeight)
                {
                    // ���õ� ���� ����� AI Character�� ���� ���¿� ����
                    chooseAttack = attack;
                    previousAttack = chooseAttack;
                    hasAttack = true;
                    return;
                }
            }
        }

        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            bool outcomeWillBePerformed = false;

            int randomPercentage = Random.Range(0, 100);

            if (randomPercentage <= outcomeChance)
            {
                outcomeWillBePerformed = true;
            }

            return outcomeWillBePerformed;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasAttack = false; // ���� �õ� ���� �ʱ�ȭ
            hasRolledForComboChance = false; // �޺� Ȯ���� �ٽ� ����� �� �ֵ��� �ʱ�ȭ

        }
    }
}