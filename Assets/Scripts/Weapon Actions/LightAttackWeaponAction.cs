using UnityEngine;

namespace SA
{
    [CreateAssetMenu(fileName ="Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponAction : WeaponItemAction
    {
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAcion, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAcion, weaponPerformingAction);
            if (!playerPerformingAcion.IsOwner)
                return;
 
            if (playerPerformingAcion.playerNetworkManager.currentStamina.Value <= 0)
                return;

            if (!playerPerformingAcion.characterLocomotionManager.isGrounded)
                return;

            PerformLightAttack(playerPerformingAcion, weaponPerformingAction);
        }

        private void PerformLightAttack(PlayerManager playerPerformingAcion, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAcion.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAcion.isPerformingAcion)
            {
                playerPerformingAcion.playerCombatManager.canComboWithMainHandWeapon = false;

                if (playerPerformingAcion.characterCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.LightAttack02, light_Attack_02, true);
                }
                else
                {
                    playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.LightAttack01, light_Attack_01, true);
                }
            }
            else if (!playerPerformingAcion.isPerformingAcion)
            {
                playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }
        }
    }
}