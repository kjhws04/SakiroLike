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

                // 기존 코드의 문제점 : (Cannot start Client while an instance is already running )
                // Shutdown()이 호출되었지만, 내부적으로 처리되는 시간이 있어서 바로 StartClient()를 호출하면 문제가 발생
                // 해결 방법 : Coroutine을 사용하여 StartClient()를 호출하기 전까지 대기
                // 기존 코드 
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