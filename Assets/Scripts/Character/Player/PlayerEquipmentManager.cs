using SA;
using UnityEngine;

namespace Sa
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;

        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandSlot;

        [SerializeField] WeaponManager rightWeaponManager;
        [SerializeField] WeaponManager leftWeaponManager;

        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();

            InitializeWeaponSlots();
        }

        protected override void Start()
        {
            base.Start();

            LoadWeaponsOnBothHands();
        }

        private void InitializeWeaponSlots()
        {
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach (var weaponSlot in weaponSlots)
            {
                if (weaponSlot.weaponSlot == WeaponModelSlot.Right_Hand)
                {
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.Left_Hand)
                {
                    leftHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();
            LoadLeftWeapon();
        }

        /// <summary>
        /// RIGHT HAND
        /// </summary>
        public void LoadRightWeapon()
        {
            if (player.playerInventoryManager.currentRightWeapon != null)
            {
                // REMOVE
                rightHandSlot.UnloadWeapon();

                // BRING NEW WEAPON
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightWeapon);
            }
        }

        public void SwitchRightHand()
        {
            if (!player.IsOwner)
                return;

            player.playerAnimationManager.PlayTargetAnimation("Swap_Right_Weapon_01", false, true, true, true);

            WeaponItem selectedWeapon = null;

            player.playerInventoryManager.rightHandSlotIndex += 1;

            if (player.playerInventoryManager.rightHandSlotIndex < 0 || player.playerInventoryManager.rightHandSlotIndex > 2)
            {
                player.playerInventoryManager.rightHandSlotIndex = 0;

                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = 0; i < player.playerInventoryManager.weaponInRightHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;

                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.rightHandSlotIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.rightHandSlotIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
                }

                return;
            }

            foreach (WeaponItem weapon in player.playerInventoryManager.weaponInRightHandSlots)
            {
                if (player.playerInventoryManager.weaponInRightHandSlots[player.playerInventoryManager.rightHandSlotIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = player.playerInventoryManager.weaponInRightHandSlots[player.playerInventoryManager.rightHandSlotIndex];

                    player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponInRightHandSlots[player.playerInventoryManager.rightHandSlotIndex].itemID;
                    return;
                }
            }

            if (selectedWeapon == null && player.playerInventoryManager.rightHandSlotIndex <= 2)
            {
                SwitchLeftHand();
            }
        }

        /// <summary>
        /// LEFT HAND
        /// </summary>
        public void LoadLeftWeapon()
        {
            if (player.playerInventoryManager.currentLeftWeapon != null)
            {
                leftHandSlot.UnloadWeapon();

                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftWeapon);
            }
        }

        public void SwitchLeftHand()
        {

        }
    }
}