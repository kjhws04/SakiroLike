using UnityEngine;
using UnityEngine.Rendering;

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

        public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
        {
            if (attackingCharacter == CharacterGroup.Joseon)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Joseon:
                        return false;
                    case CharacterGroup.Monster:
                        return true;
                    case CharacterGroup.Japan:
                        return true;
                    default:
                        break;
                }
            }
            else if (attackingCharacter == CharacterGroup.Japan)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Joseon:
                        return true;
                    case CharacterGroup.Monster:
                        return true;
                    case CharacterGroup.Japan:
                        return false;
                    default:
                        break;
                }
            }
            else if (attackingCharacter == CharacterGroup.Monster)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Joseon:
                        return true;
                    case CharacterGroup.Monster:
                        return false;
                    case CharacterGroup.Japan:
                        return true;
                    default:
                        break;
                }
            }

            return false;
        }

        public float GetAngleOfTarget(Transform Charactertransform, Vector3 targetDirection)
        {
            targetDirection.y = 0;
            float viewableAngle = Vector3.Angle(Charactertransform.forward, targetDirection);
            Vector3 cross = Vector3.Cross(Charactertransform.forward, targetDirection);

            if (cross.y < 0) 
            {
                viewableAngle = -viewableAngle;
            }

            return viewableAngle;
        }
    }
}