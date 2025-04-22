using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
    public class WeaponItemAction : ScriptableObject
    {
        public int weaponID;
        public int actionID;

        public virtual void AttemptToPerformAction(PlayerManager playerPerformAction, WeaponItem weaponPerformAction)
        {
            if (playerPerformAction.IsOwner)
            {
                playerPerformAction.playerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformAction.itemID;
            }

            Debug.Log("THE ACTION WAS FIRED");
        }
    }
}