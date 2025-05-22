using Mono.Cecil.Cil;
using UnityEngine;

namespace SA
{
    public class TitleScreenLoadMenuInputManager : MonoBehaviour
    {
        PlayerControls playerControls;

        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false;

        private void Update()
        {
            if (deleteCharacterSlot)
            {
                deleteCharacterSlot = false;
                TitleScreenManager.instance.AttempToDeleteCharacterSlot();              
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.UI.Enter.performed += ctx => deleteCharacterSlot = true;
            }

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}