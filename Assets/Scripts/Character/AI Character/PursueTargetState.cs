using UnityEngine;
using UnityEngine.AI;

namespace SA
{
    [CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // 1. 캐릭터가 현재 액션을 수행중이면 대기
            if (aiCharacter.isPerformingAcion)
                return this;

            // 2. 추격 대상이 존재하지 않으면 서서시 idle 상태로 전환
            if (aiCharacter.characterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // 3. Navmesh가 비활성화 상태이면 활성화
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // 대상이 캐릭터의 시야각(FOV)를 벗어나면 대상 character을 향해 몸돌림
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV 
                || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
                aiCharacter.aiCharacterCombatManager.PivotTowarsTarget(aiCharacter);

            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // 4. 대상이 전투 가능 범위 안에 있으면 combat 상태로 전환

            // 5. 대상에게 도달할 수 없거나 멀리 있으면 원래 위치로 돌아감

            // 6. 그렇지 않으면 계속해서 대상을 추적

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
    }
}