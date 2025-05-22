using UnityEngine;
using UnityEngine.AI;

namespace SA
{
    [CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // 1. ĳ���Ͱ� ���� �׼��� �������̸� ���
            if (aiCharacter.isPerformingAcion)
                return this;

            // 2. �߰� ����� �������� ������ ������ idle ���·� ��ȯ
            if (aiCharacter.characterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // 3. Navmesh�� ��Ȱ��ȭ �����̸� Ȱ��ȭ
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // ����� ĳ������ �þ߰�(FOV)�� ����� ��� character�� ���� ������
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV 
                || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
                aiCharacter.aiCharacterCombatManager.PivotTowarsTarget(aiCharacter);

            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // 4. ����� ���� ���� ���� �ȿ� ������ combat ���·� ��ȯ

            // 5. ��󿡰� ������ �� ���ų� �ָ� ������ ���� ��ġ�� ���ư�

            // 6. �׷��� ������ ����ؼ� ����� ����

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
    }
}