using UnityEngine;

namespace SA
{
    [CreateAssetMenu(fileName ="Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponAction : WeaponItemAction
    {
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";
        [SerializeField] string light_Attack_03 = "Main_Light_Attack_03";
        [SerializeField] string light_Attack_04 = "Main_Light_Attack_04";
        [SerializeField] string light_Attack_05 = "Main_Light_Attack_05";

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

        private void PerformLightAttack(PlayerManager player, WeaponItem weapon)
        {
            var combatManager = player.playerCombatManager;
            var animationManager = player.playerAnimationManager;
            string lastAttack = player.characterCombatManager.lastAttackAnimationPerformed;

            if (combatManager.canComboWithMainHandWeapon && player.isPerformingAcion)
            {
                combatManager.canComboWithMainHandWeapon = false;

                if (lastAttack == light_Attack_01)
                    animationManager.PlayTargetAttackAnimation(AttackType.LightAttack03, light_Attack_02, true);
                else if (lastAttack == light_Attack_02)
                    animationManager.PlayTargetAttackAnimation(AttackType.LightAttack03, light_Attack_03, true);
                else if (lastAttack == light_Attack_03)
                    animationManager.PlayTargetAttackAnimation(AttackType.LightAttack04, light_Attack_04, true);
                else if (lastAttack == light_Attack_04)
                    animationManager.PlayTargetAttackAnimation(AttackType.LightAttack05, light_Attack_05, true);
                else
                    animationManager.PlayTargetAttackAnimation(AttackType.LightAttack02, light_Attack_01, true); // fallback
            }
            else if (!player.isPerformingAcion)
            {
                animationManager.PlayTargetAttackAnimation(AttackType.LightAttack01, light_Attack_01, true, false);
            }
        }
    }
}