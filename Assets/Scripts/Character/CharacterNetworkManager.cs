using UnityEngine;
using Unity.Netcode;

namespace SA
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager character;

        [Header("Position")]

        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;

        [Header("Animator")]
        public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animatorHorizontalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animatorVerticalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animatorMoveAmount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Target")]
        public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isLockOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isHoldAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Resources")]
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }   

        public void CheckHp(int oldValue, int newValue)
        {
            if (currentHealth.Value <= 0)
            {
                StartCoroutine(character.PrecessDeathEvent());
            }

            if (character.IsOwner)
            {
                if (currentHealth.Value > maxHealth.Value)
                {
                    currentHealth.Value = maxHealth.Value;
                }
            }
        }

        public void OnLockOnTargetIDChange(ulong oldID, ulong newID)
        {
            if (!IsOwner)
            {
                character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
            }
        }

        public void OnIsLockedOnChanged(bool old, bool isLockOn)
        {
            if (!isLockOn)
            {
                character.characterCombatManager.currentTarget = null;
            }
        }

        public void OnIsHoldAttackChanged(bool oldState, bool newState)
        {
            character.anim.SetBool("IsHoldAttack", isHoldAttack.Value);
        }

        public void OnIsMovingChanged(bool oldState, bool newState)
        {
            character.anim.SetBool("IsMoving", isMoving.Value);
        }

        // ���� RPC�� Ŭ���̾�Ʈ���� ������ ȣ��Ǵ� �Լ� (����� ȣ��Ʈ)
        [ServerRpc]
        public void NotifyTheServerOfAcionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // �� ĳ���Ͱ� ȣ��Ʈ/�������, Ŭ���̾�Ʈ RPC�� Ȱ��ȭ
            if (IsServer)
            {
                PlayAcionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);    
            }
        }

        // Ŭ���̾�Ʈ RPC�� �������� ��� Ŭ���̾�Ʈ�� ���۵Ǵ� �Լ�
        [ClientRpc]
        public void PlayAcionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // �� �Լ��� ���� ĳ���Ϳ��� �������� �ʵ��� �ϱ� ���� Ȯ�� (�ִϸ��̼��� �� �� ��� ����)
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAcionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformAcionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.characterAnimationManager.applyRootMotion = applyRootMotion;
            character.anim.CrossFade(animationID, 0.2f);
        }

        /// ���� �ִϸ��̼�

        // ���� RPC�� Ŭ���̾�Ʈ���� ������ ȣ��Ǵ� �Լ� (����� ȣ��Ʈ)
        [ServerRpc]
        public void NotifyTheServerOfAttackAcionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // �� ĳ���Ͱ� ȣ��Ʈ/�������, Ŭ���̾�Ʈ RPC�� Ȱ��ȭ
            if (IsServer)
            {
                PlayAttackAcionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        // Ŭ���̾�Ʈ RPC�� �������� ��� Ŭ���̾�Ʈ�� ���۵Ǵ� �Լ�
        [ClientRpc]
        public void PlayAttackAcionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // �� �Լ��� ���� ĳ���Ϳ��� �������� �ʵ��� �ϱ� ���� Ȯ�� (�ִϸ��̼��� �� �� ��� ����)
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAttackAcionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformAttackAcionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.characterAnimationManager.applyRootMotion = applyRootMotion;
            character.anim.CrossFade(animationID, 0.2f);
        }

        /// ������
        [ServerRpc(RequireOwnership = false)]
        public void NotifyTheServerOfCharacterDamageServerRpc(
            ulong damageCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            if (IsServer)
            {
                NotifyTheServerOfCharacterDamageClientRpc(damageCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, lightningDamage, holyDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
            }
        }

        [ClientRpc]
        public void NotifyTheServerOfCharacterDamageClientRpc(
            ulong damageCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            ProcessCharacterDamageFromServer(damageCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, lightningDamage, holyDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }

        public void ProcessCharacterDamageFromServer(
            ulong damageCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damageCharacterID].gameObject.GetComponent<CharacterManager>();
            CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

            TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);

            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.angleHitFrom = angleHitFrom;
            damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
            damageEffect.characterCausingDamage = characterCausingDamage;

            damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }
    }
}