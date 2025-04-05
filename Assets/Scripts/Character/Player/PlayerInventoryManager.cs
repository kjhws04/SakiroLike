using UnityEngine;

namespace SA
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public WeaponItem currentRightWeapon;
        public WeaponItem currentLeftWeapon;

        [Header("Quick Slots")]
        public WeaponItem[] weaponInRightHandSlots = new WeaponItem[3];
        public int rightHandSlotIndex = 0;
        public WeaponItem[] weaponInLeftHandSlots = new WeaponItem[3];
        public int leftHandSlotIndex = 0;
    }
}