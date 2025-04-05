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
        public NetworkVariable<float> animatorHorizontalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animatorVerticalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animatorMoveAmount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

        // 서버 RPC는 클라이언트에서 서버로 호출되는 함수 (현재는 호스트)
        [ServerRpc]
        public void NotifyTheServerOfAcionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // 이 캐릭터가 호스트/서버라면, 클라이언트 RPC를 활성화
            if (IsServer)
            {
                PlayAcionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);    
            }
        }

        // 클라이언트 RPC는 서버에서 모든 클라이언트로 전송되는 함수
        [ClientRpc]
        public void PlayAcionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // 이 함수를 보낸 캐릭터에서 실행하지 않도록 하기 위해 확인 (애니메이션을 두 번 재생 방지)
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAcionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformAcionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.anim.CrossFade(animationID, 0.2f);
        }
    }
}