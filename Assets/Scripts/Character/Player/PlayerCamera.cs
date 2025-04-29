using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace SA
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTransform;

        [Header("Camera Setting")]
        private float cameraSmoothTime = 1f; //숫자가 클수록 카메라가 움직일 때, 그 위치에 도달하는 데 더 오래 걸림
        [SerializeField] float leftAndRightRotationSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float miniumPivotAngle = -30;
        [SerializeField] float maximumPivotAngle = 60;
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] LayerMask collideWithLayers;

        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition;
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float cameraZPosition;
        private float targetCameraZPosition;

        [Header("Lock On")]
        [SerializeField] float lockOnRaidius = 20f;
        [SerializeField] float minimumViewableAngle = -50;
        [SerializeField] float maximumViewableAngle = 50f;
        [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
        [SerializeField] float setCameraHeightSpeed = 0.05f;
        [SerializeField] float inlockedCameraHeight = 1.65f;
        [SerializeField] float lockedCameraHeight = 2.0f;
        private Coroutine cameraLockOnHeightCoroutine;
        private List<CharacterManager> availbleTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockOnTarget;
        public CharacterManager rightLockOnTarget;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                FollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void FollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothTime * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            if (player.playerNetworkManager.isLockOn.Value)
            {
                Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

                rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
                rotationDirection.Normalize();

                targetRotation = Quaternion.LookRotation(rotationDirection);
                cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

                leftAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = transform.eulerAngles.x;
            }
            else
            {
                leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
                upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
                upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, miniumPivotAngle, maximumPivotAngle);

                Vector3 cameraRotation = Vector3.zero;
                Quaternion targetRotation;

                cameraRotation.y = leftAndRightLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                transform.rotation = targetRotation;

                cameraRotation = Vector3.zero;
                cameraRotation.x = upAndDownLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }

        public void HandleLocatingLockOnTargets()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRaidius, WorldUtillityManager.instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                if (lockOnTarget != null)
                {
                    Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                    float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetsDirection, cameraObject.transform.forward);
                    
                    // 필터링 : 캐릭터가 죽었는지 확인, 자신과의 거리 확인, 시야각 확인, 장애물 확인
                    if (lockOnTarget.isDead.Value)
                        continue;

                    if (lockOnTarget.transform.root == player.transform.root)
                        continue;

                    // 타켓들을 List에 추가
                    if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                    {
                        RaycastHit hit;

                        if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, 
                            lockOnTarget.characterCombatManager.lockOnTransform.position, 
                            out hit,
                            WorldUtillityManager.instance.GetEnviroLayers()))
                        {
                            continue;
                        }
                        else
                        {
                            availbleTargets.Add(lockOnTarget);
                        }
                    }
                }
            }

            for (int k = 0; k < availbleTargets.Count; k++)
            {
                if (availbleTargets[k] != null)
                {
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availbleTargets[k].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availbleTargets[k];
                    }

                    if (player.playerNetworkManager.isLockOn.Value)
                    {
                        Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availbleTargets[k].transform.position);
                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        if (availbleTargets[k] == player.playerCombatManager.currentTarget)
                            continue;

                        // 왼쪽 타겟을 확인
                        if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockOnTarget = availbleTargets[k];
                        }
                        // 오른쪽 타겟을 확인
                        else if (relativeEnemyPosition.x > 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockOnTarget = availbleTargets[k];
                        }
                    }
                }
                else
                {
                    ClearLockOnTarget();
                    player.playerNetworkManager.isLockOn.Value = false; 
                }
            }

        }

        public void SetLockCameraHeight()
        {
            if (cameraLockOnHeightCoroutine != null)
            {
                StopCoroutine(cameraLockOnHeightCoroutine);
            }

            cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
        }

        public void ClearLockOnTarget()
        {
            nearestLockOnTarget = null;
            leftLockOnTarget = null;
            rightLockOnTarget = null;
            availbleTargets.Clear();
        }

        public IEnumerator WaitThenFindNewTarget()
        {
            while (player.isPerformingAcion)
            {
                yield return null;
            }

            ClearLockOnTarget();
            HandleLocatingLockOnTargets();

            if (nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(nearestLockOnTarget);
                player.playerNetworkManager.isLockOn.Value = true;
            }

            yield return null;
        }

        private IEnumerator SetCameraHeight()
        {
            float duration = 1;
            float timer = 0;

            Vector3 velocity = Vector3.zero;
            Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
            Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, inlockedCameraHeight);

            while (timer < duration)
            {
                timer += Time.deltaTime;

                if (player != null)
                {
                    if (player.playerCombatManager.currentTarget != null)
                    {
                        cameraPivotTransform.transform.localPosition =
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                        cameraPivotTransform.transform.localRotation =
                            Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                    }
                    else
                    {
                        cameraPivotTransform.transform.localPosition =
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    }
                }

                yield return null;
            }

            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
                }
            }

            yield return null;
        }
    }
}