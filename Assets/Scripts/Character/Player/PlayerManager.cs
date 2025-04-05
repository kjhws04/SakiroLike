using Sa;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SA
{
    public class PlayerManager : CharacterManager
    {
        [Header("Debug Menu")]
        [SerializeField] bool respawnPlayer = false;
        [SerializeField] bool switchRightWeapon = false;    

        [HideInInspector] public PlayerAnimationManager playerAnimationManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;

        protected override void Awake()
        {
            base.Awake();

            playerAnimationManager = GetComponent<PlayerAnimationManager>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
        }

        protected override void Update()
        {
            base.Update();
             
            // �� ���� ������Ʈ�� �������� �ʾҴٸ�, �����ϰų� �������� ����
            if (!IsOwner)
                return;

            // Movement
            playerLocomotionManager.HandleAllMovement();

            // Regen Stmina
            playerStatsManager.RegenerateStamina();

            DebugMenu();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner)
                return;

            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                // ����� ������ ����� �� ��ü ü�� �Ǵ� ���׹̳� ���� ������Ʈ
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetMaxNewStaminaValue;

                // ������ ����� �� UI ���� �ٸ� ������Ʈ(ü��, ���׹̳�)
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            }

            // ����
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHp;

            // ����
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

            // ����(spwin)�Ǿ��� ��, ĳ������ ������������ ������ �ƴ� ���, ���� ������ ĳ���Ϳ� ĳ���� �����͸� �ٽ� �ε�
            // ������ ȣ��Ʈ�� ������ �̹� �ε�Ǿ� �־� �����͸� �ٽ� �ε��� �ʿ䰡 ����

            if (IsOwner && !IsServer)
            {
                LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }
        }

        public override IEnumerator PrecessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }

            return base.PrecessDeathEvent(manuallySelectDeathAnimation);
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            if (IsOwner)
            {
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                // ü��, ���׹̳� UI ������Ʈ

                playerAnimationManager.PlayTargetAnimation("Empty", false);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // ��ġ ���� ���̺�
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;
            currentCharacterData.xRotation = transform.rotation.eulerAngles.x;
            currentCharacterData.yRotation = transform.rotation.eulerAngles.y;
            currentCharacterData.zRotation = transform.rotation.eulerAngles.z;

            // ü��, ���׹̳� ���̺�
            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamia = playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            // ��ġ ���� �ε�
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPos = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPos;
            Quaternion myRot = Quaternion.Euler(currentCharacterData.xRotation, currentCharacterData.yRotation, currentCharacterData.zRotation);
            transform.rotation = myRot;

            // ü��, ���׹̳� �ε�
            playerNetworkManager.vitality.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;

            // ���� �� �ε尡 �߰��� �� �̵�
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamia;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }   

        // TODO : Debug Menu
        private void DebugMenu()
        {
            if (respawnPlayer)
            {
                respawnPlayer = false;
                ReviveCharacter();
            }

            if (switchRightWeapon)
            {
                switchRightWeapon = false;
                playerEquipmentManager.SwitchRightHand();
            }
        }
    }
}