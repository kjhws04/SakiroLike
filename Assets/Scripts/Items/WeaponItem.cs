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

        [Header("Attack Modifiers")]
        public float light_Attack_01_Modifier = 1.1f;
        public float heavy_Attack_01_Modifier = 1.4f;
        public float hold_Attack_01_Modifier = 2.0f;

        [Header("Stamina Cost Modifiers")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCost = 0.9f;

        [Header("Actions")]
        public WeaponItemAction lightAttackAction;
        public WeaponItemAction HeavyAttackAction;
    }
}