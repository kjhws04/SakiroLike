using UnityEngine;

namespace SA
{
    public class AIChangguiCombatManager : AICharacterCombatManager
    {
        [Header("Changgui Damage Collider")]
        [SerializeField] ChangguiDamageCollider rightHandDamageCollider;
        [SerializeField] ChangguiDamageCollider leftHandDamageCollider;

        [Header("Damage")]
        [SerializeField] float baseDamage = 25;
        [SerializeField] float attack01DamageModifier = 1.0f;
        [SerializeField] float attack02DamageModifier = 2.0f;

        #region Animation Events (editing Careful)
        public void SetAttack01Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }

        public void SetAttack02Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        }

        public void EnableRightHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void DisableRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void EnableLeftHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void DisableLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }
        #endregion
    }
}