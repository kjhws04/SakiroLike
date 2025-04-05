using UnityEngine;

namespace SA
{
    public class WeaponItem : Items
    {
        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requrtements")]
        public int strengthREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;
        public int faithREQ = 0;

        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int holyDamage = 0;

        [Header("Weapon Poise")]
        public float poiseDamage = 10;

        [Header("Stamina Cost")]
        public int baseStaminaCost = 20;

        [Header("Actions")]
        public WeaponItemAction rbAction;
    }
}