using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

namespace SA
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        CharacterManager character;

        int horizontal;
        int vertical;

        [Header("Flags")]
        public bool applyRootMotion = false;

        [Header("Damage Animations")]
        public string lastDamageAnimationPlayed;

        [Header("Damage Animations")]
        [SerializeField] string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
        [SerializeField] string hit_Forward_Medium_02 = "hit_Forward_Medium_02";
        [SerializeField] string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
        [SerializeField] string hit_Backward_Medium_02 = "hit_Backward_Medium_02";
        [SerializeField] string hit_Right_Medium_01 = "hit_Right_Medium_01";
        [SerializeField] string hit_Right_Medium_02 = "hit_Right_Medium_02";
        [SerializeField] string hit_Left_Medium_01 = "hit_Left_Medium_01";
        [SerializeField] string hit_Left_Medium_02 = "hit_Left_Medium_02";

        public List<string> forward_Medium_Damage = new List<string>();
        public List<string> backward_Medium_Damage = new List<string>();
        public List<string> right_Medium_Damage = new List<string>();
        public List<string> left_Medium_Damage = new List<string>();

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            horizontal = Animator.StringToHash("Horizontal");
            vertical = Animator.StringToHash("Vertical");
        }

        protected virtual void Start()
        {
            forward_Medium_Damage.Add(hit_Forward_Medium_01);
            forward_Medium_Damage.Add(hit_Forward_Medium_02);

            backward_Medium_Damage.Add(hit_Backward_Medium_01);
            backward_Medium_Damage.Add(hit_Backward_Medium_02);

            right_Medium_Damage.Add(hit_Right_Medium_01);
            right_Medium_Damage.Add(hit_Right_Medium_02);

            left_Medium_Damage.Add(hit_Left_Medium_01);
            left_Medium_Damage.Add(hit_Left_Medium_02);
        }

        public string GetRandomAnimationFromList(List<string> animationList)
        {
            List<string> finalList = new List<string>();

            foreach (var item in animationList)
            {
                finalList.Add(item);
            }

            finalList.Remove(lastDamageAnimationPlayed);

            for (int i = finalList.Count - 1; i > -1; i--)
            {
                if (finalList[i] == null)
                {
                    finalList.RemoveAt(i);
                    break;
                }
            }

            int randomIndex = Random.Range(0, finalList.Count);

            return finalList[randomIndex];
        }

        public void UpdateanimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            float snappedHorizontalAmout;
            float snappedVerticalAmount;

            if (horizontalValue > 0 && horizontalValue <= 0.5f)
            {
                snappedHorizontalAmout = 0.5f;
            }
            else if (horizontalValue > 0.5f && horizontalValue <= 1f)
            {
                snappedHorizontalAmout = 1f;
            }
            else if (horizontalValue < 0 && horizontalValue >= -0.5f)
            {
                snappedHorizontalAmout = -0.5f;
            }
            else if (horizontalValue < -0.5f && horizontalValue >= -1f)
            {
                snappedHorizontalAmout = -1f;
            }
            else
            {
                snappedHorizontalAmout = 0f;
            }

            if (verticalValue > 0 && verticalValue <= 0.5f)
            {
                snappedVerticalAmount = 0.5f;
            }
            else if (verticalValue > 0.5f && verticalValue <= 1f)
            {
                snappedVerticalAmount = 1f;
            }
            else if (verticalValue < 0 && verticalValue >= -0.5f)
            {
                snappedVerticalAmount = -0.5f;
            }
            else if (verticalValue < -0.5f && verticalValue >= -1f)
            {
                snappedVerticalAmount = -1f;
            }
            else
            {
                snappedVerticalAmount = 0f;
            }

            if (isSprinting)
            {
                snappedVerticalAmount = 2f;
            }

            character.anim.SetFloat(horizontal, snappedHorizontalAmout, 0.1f, Time.deltaTime);
            character.anim.SetFloat(vertical, snappedVerticalAmount, 0.1f, Time.deltaTime);

        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation, 
            bool isPerformingAction, 
            bool applyRootMotion = true, 
            bool canRotate = false, 
            bool canMove = false)
        {
            Debug.Log($"Playing Animation : {targetAnimation}");

            character.anim.applyRootMotion = applyRootMotion;
            character.anim.CrossFade(targetAnimation, 0.2f);

            // 캐릭터가 새로운 액션을 시도하는 것을 막기 위해 사용. 예를 들어, 피해 애니메이션을 시작하면 이 플래그가 true가 됨
            character.isPerformingAcion = isPerformingAction;
            character.characterAnimationManager.applyRootMotion = applyRootMotion;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;

            character.characterNetworkManager.NotifyTheServerOfAcionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public virtual void PlayTargetAttackAnimation(AttackType attackTpye,
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            character.characterCombatManager.currentAttackType = attackTpye;
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;

            character.anim.applyRootMotion = applyRootMotion;
            character.anim.CrossFade(targetAnimation, 0.2f);

            // 캐릭터가 새로운 액션을 시도하는 것을 막기 위해 사용. 예를 들어, 피해 애니메이션을 시작하면 이 플래그가 true가 됨
            character.isPerformingAcion = isPerformingAction;
            character.characterAnimationManager.applyRootMotion = applyRootMotion;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;

            character.characterNetworkManager.NotifyTheServerOfAcionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public virtual void EnableCanDoCombo()
        {

        }

        public virtual void DisableCanDoCombo()
        {

        }
    }
}