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
             
            // 이 게임 오브젝트를 소유하지 않았다면, 제어하거나 편집하지 않음
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

                // 연결된 스탯이 변경될 때 전체 체력 또는 스테미너 양을 업데이트
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetMaxNewStaminaValue;

                // 스탯이 변경될 때 UI 스탯 바를 업데이트(체력, 스테미너)
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            }

            // 스텟
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHp;

            // 무기
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

            // 연결(spwin)되었을 때, 캐릭터의 소유자이지만 서버가 아닌 경우, 새로 생성된 캐릭터에 캐릭터 데이터를 다시 로드
            // 하지만 호스트인 서버는 이미 로드되어 있어 데이터를 다시 로드할 필요가 없음

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
                // 체력, 스테미너 UI 업데이트

                playerAnimationManager.PlayTargetAnimation("Empty", false);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // 위치 방향 세이브
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;
            currentCharacterData.xRotation = transform.rotation.eulerAngles.x;
            currentCharacterData.yRotation = transform.rotation.eulerAngles.y;
            currentCharacterData.zRotation = transform.rotation.eulerAngles.z;

            // 체력, 스테미너 세이브
            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamia = playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            // 위치 방향 로드
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPos = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPos;
            Quaternion myRot = Quaternion.Euler(currentCharacterData.xRotation, currentCharacterData.yRotation, currentCharacterData.zRotation);
            transform.rotation = myRot;

            // 체력, 스테미너 로드
            playerNetworkManager.vitality.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;

            // 저장 및 로드가 추가될 때 이동
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