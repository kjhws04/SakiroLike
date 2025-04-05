using UnityEngine;

namespace SA
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager instance;

        [Header("Action Sound")]
        public AudioClip rollSFX;

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
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}