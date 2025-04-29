using UnityEngine;
using Unity.Netcode;

namespace SA
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;

        public WeaponItem currentWeaponBeingUsed;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAcion)
        {
            if (player.IsOwner)
            {
                // �ൿ�� ����
                weaponAction.AttemptToPerformAction(player, weaponPerformingAcion);

                // �������� �ൿ�� ���������� �˸�. ����� ���������� ����.
                player.playerNetworkManager.NotifyTheServerOfAcionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAcion.itemID);
            }
        }

        public virtual void DrainStaminaBasedOnAttack()
        {
            if (!player.IsOwner)
                return;
            
            if (currentWeaponBeingUsed == null)
                return;

            float staminaDeducted = 0f;

            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCost;
                    break;
                default:
                    break;
            }

            Debug.Log("Stamina Deducted: " + staminaDeducted);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

        public override void SetTarget(CharacterManager newTarget)
        {
            base.SetTarget(newTarget);

            if (player.IsOwner)
            {
                PlayerCamera.instance.SetLockCameraHeight();
            }
        }
    }
}
