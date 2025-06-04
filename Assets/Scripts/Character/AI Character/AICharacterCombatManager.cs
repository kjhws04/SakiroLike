using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows.Speech;

namespace SA
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        protected AICharacterManager aiCharacter;

        [Header("Action Recovery")]
        public float actionRecoveryTime = 0f;

        [Header("Target Information")]
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetsDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 15f;
        public float minimumFOV = -35f;
        public float maximumFOV = 35f;

        [Header("Attack Rotation Speed")]
        public float attackRotationSpeed = 25f;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
            lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
        }

        public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;

            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtillityManager.instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].GetComponent<CharacterManager>();

                if (targetCharacter == null)
                    continue;

                if (targetCharacter == aiCharacter)
                    continue;

                if (targetCharacter.isDead.Value)
                    continue;

                if (WorldUtillityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
                {
                    Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angleOfPotentalTarget = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                    if (angleOfPotentalTarget < maximumFOV && angleOfPotentalTarget > minimumFOV)
                    {
                        if (Physics.Linecast(
                            aiCharacter.characterCombatManager.lockOnTransform.position, 
                            targetCharacter.characterCombatManager.lockOnTransform.position, 
                            WorldUtillityManager.instance.GetEnviroLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                        }
                        else
                        {
                            targetDirection = targetCharacter.transform.position - transform.position;
                            viewableAngle = WorldUtillityManager.instance.GetAngleOfTarget(transform, targetDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            PivotTowarsTarget(aiCharacter);
                        }
                    }
                }
            }
        }

        public void PivotTowarsTarget(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAcion)
                return;

            if (viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Right_045", true);
            }
            else if (viewableAngle <= -20 && viewableAngle >= -60)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Left_045", true);
            }

            if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Right_090", true);
            }
            else if (viewableAngle <= -61 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Left_090", true);
            }

            if (viewableAngle >= 111 && viewableAngle <= 160)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Right_135", true);
            }
            else if (viewableAngle <= -111 && viewableAngle >= -160)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Left_135", true);
            }

            if (viewableAngle >= 161 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if (viewableAngle <= -161 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }
        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }

        public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null)
                return;

            if (!aiCharacter.characterLocomotionManager.canRotate)
                return;

            if (!aiCharacter.isPerformingAcion)
                return;

            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0f; // Y축 회전을 방지하기 위해 Y값을 0으로 설정
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
        }

        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTime > 0f)
            {
                if (aiCharacter.isPerformingAcion)
                {
                    actionRecoveryTime -= Time.deltaTime;
                }
            }
        }
    }
}