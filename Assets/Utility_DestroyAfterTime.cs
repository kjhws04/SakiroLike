using UnityEngine;

namespace SA
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float timeUnitlDestroyed = 5;

        private void Awake()
        {
            Destroy(gameObject, timeUnitlDestroyed);
        }
    }
}