using UnityEngine;

namespace SA
{
    public class WorldUtillityManager : MonoBehaviour
    {
        public static WorldUtillityManager instance;

        [Header("Layers")]
        [SerializeField] LayerMask characterLayers;
        [SerializeField] LayerMask enviroLayers;

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

        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }

        public LayerMask GetEnviroLayers()
        {
            return enviroLayers;
        }
    }
}