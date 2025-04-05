using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SA
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;

        public WeaponItem unarmedWeapon;

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        [Header("Items")]
        private List<Items> items = new List<Items>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }

            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponByID(int id)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == id);
        }
    }
}