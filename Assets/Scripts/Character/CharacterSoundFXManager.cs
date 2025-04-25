using UnityEngine;

namespace SA
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        private AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool ramdomizePitch = true, float pitchRamdom = 0.1f)
        {
            audioSource.PlayOneShot(soundFX, volume);

            audioSource.pitch = 1;

            if (ramdomizePitch)
            {
                audioSource.pitch += Random.Range(-pitchRamdom, pitchRamdom);
            }
        }
        
        public void PlayRollSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
        }
    }
}