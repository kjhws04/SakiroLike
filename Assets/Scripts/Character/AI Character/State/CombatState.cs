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
        // 각 AI 캐릭터는 고유한 공격 리스트를 가짐
        [Header("Attacks")]
        public List<AICharacterAttackAction> aiCharacterAttacks; // 전투 상황에서 후보군이 되는 공격 기술들의 총 집합
        protected List<AICharacterAttackAction> potentialAttacks; // 실제로 사용할 수 있는 공격만 필터링 한 집합 (시야각, 거리, 상태 등등)
        private AICharacterAttackAction chooseAttack; // 선택된 공격 기술
        private AICharacterAttackAction previousAttack; // 이전에 선택된 공격 기술
        protected bool hasAttack = false; // AI가 공격을 시도했는지 여부

        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false; // 콤보를 수행할 수 있는지 여부
        [SerializeField] protected int chanceToPerformCombo = 25; // 콤보를 시도 확률 (0~100 사이의 값)
        protected bool hasRolledForComboChance = false; // 콤보를 수행할 확률을 계산했는지 여부

        [Header("Engagement Distance")]
        [SerializeField] public float maxiumEngagementDistance = 5f; // AI가 공격을 시도할 수 있는 최대 거리

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAcion)
                return this;

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // AI 캐릭터가 시야각(FOV) 밖에 있을 때 타겟을 바라봄
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    aiCharacter.aiCharacterCombatManager.PivotTowarsTarget(aiCharacter);
            }

            // 타겟을 향해 회전
            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            // 타켓이 없으면 idle 상태로 전환
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // 공격이 선택되지 않았다면, 다시 공격을 선택
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = chooseAttack;
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            // 공격 가능 범위보다 멀리 있으면 추적 상태(pursueTarget)로 전환
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

            // 1. 가능한 모든 공격 기술을 조회 (현재 조건에 적합하지 않은 공격은 제거)
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

                // 남은 공격 기술을 potentialAttack에 저장
                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0)
            {
                Debug.Log("No potential attacks available for the AI character.");
                return;
            }

            // 2. 가중치 기반 랜덤 선택
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
                    // 선택된 공격 기술을 AI Character의 공격 상태에 전달
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

            hasAttack = false; // 공격 시도 여부 초기화
            hasRolledForComboChance = false; // 콤보 확률을 다시 계산할 수 있도록 초기화

        }
    }
}