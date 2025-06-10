using UnityEngine;
using Unity.Netcode;


namespace SA
{
    public class FogWallInteractable : NetworkBehaviour
    {
        [Header("Fog")]
        [SerializeField] GameObject[] fogWallObjects;

        [Header("I.D")]
        public int fogWallID;

        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            OnIsActiveChanged(false, isActive.Value);
            isActive.OnValueChanged += OnIsActiveChanged;
            WorldObjectManager.instance.AddFogWallToList(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            isActive.OnValueChanged -= OnIsActiveChanged;
            WorldObjectManager.instance.RemoveFogWallToList(this);
        }

        private void OnIsActiveChanged(bool oldStatus, bool newStatus)
        {
            if (isActive.Value)
            {
                foreach(var fogWallObject in fogWallObjects)
                {
                    fogWallObject.SetActive(true);
                }
            }
            else
            {
                foreach(var fogWallObject in fogWallObjects)
                {
                    fogWallObject.SetActive(false);
                }
            }
        }
    }
}