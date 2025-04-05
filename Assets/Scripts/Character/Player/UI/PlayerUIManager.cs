using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace SA
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;

        [Header("Network Join")]
        [SerializeField] bool startGameAsClient;

        [HideInInspector] public PlayerUIHudManager playerUIHudManager;
        [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;

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

            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
            playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;

                StartCoroutine(RestartClient());

                // ���� �ڵ��� ������ : (Cannot start Client while an instance is already running )
                // Shutdown()�� ȣ��Ǿ�����, ���������� ó���Ǵ� �ð��� �־ �ٷ� StartClient()�� ȣ���ϸ� ������ �߻�
                // �ذ� ��� : Coroutine�� ����Ͽ� StartClient()�� ȣ���ϱ� ������ ���
                // ���� �ڵ� 
                // NetworkManager.Singleton.StartClient();
                // NetworkManager.Singleton.Shutdown();
            }
        }

        private IEnumerator RestartClient()
        {
            if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
                yield return new WaitUntil(() => NetworkManager.Singleton.ShutdownInProgress);
            }

            if (!NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}