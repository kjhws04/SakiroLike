using UnityEngine;
using Unity.Netcode;

namespace SA
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        CharacterManager character;

        int horizontal;
        int vertical;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            horizontal = Animator.StringToHash("Horizontal");
            vertical = Animator.StringToHash("Vertical");
        }

        public void UpdateanimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            float horizontalAmout = horizontalValue;
            float verticalAmount = verticalValue;

            if (isSprinting)
            {
                verticalAmount = 2f;
            }

            character.anim.SetFloat(horizontal, horizontalAmout, 0.1f, Time.deltaTime);
            character.anim.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);

        }

        public virtual void PlayTargetAnimation(
            string targetAnimation, 
            bool isPerformingAction, 
            bool applyRootMotion = true, 
            bool canRotate = false, 
            bool canMove = false)
        {
            character.anim.applyRootMotion = applyRootMotion;
            character.anim.CrossFade(targetAnimation, 0.2f);

            // 캐릭터가 새로운 액션을 시도하는 것을 막기 위해 사용. 예를 들어, 피해 애니메이션을 시작하면 이 플래그가 true가 됨
            character.isPerformingAcion = isPerformingAction;
            character.applyRootMotion = applyRootMotion;
            character.canRotate = canRotate;
            character.canMove = canMove;

            character.characterNetworkManager.NotifyTheServerOfAcionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
    }
}