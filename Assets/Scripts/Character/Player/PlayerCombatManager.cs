using UnityEngine;

namespace SA
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;

        public WeaponItem currentWeaponItem;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAcion)
        {
            // 행동을 수행
            weaponAction.AttemptToPerformAction(player, weaponPerformingAcion);

            // 서버에게 행동을 수행했음을 알림. 대상의 관점에서도 수행.
        }
    }
}