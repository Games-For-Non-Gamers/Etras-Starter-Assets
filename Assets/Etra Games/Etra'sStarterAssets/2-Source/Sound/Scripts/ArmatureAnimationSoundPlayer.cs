using UnityEngine;

namespace EtrasStarterAssets
{
    public class ArmatureAnimationSoundPlayer : MonoBehaviour
    {
        private GameObject _mainCamera;
        private AudioManager abilitySoundManager;
        private AudioManager foostepSoundManager;
        void Start()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = _mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
            foostepSoundManager = _mainCamera.transform.Find("FootstepsAudio").GetComponent<AudioManager>();
        }

        public void PlayJumpSound()
        {
            abilitySoundManager.Play("Jump");
        }

        public void PlayLandSound()
        {
            abilitySoundManager.Play("Land");
        }

        int stepSoundCount = 0;
        public void PlayFootstep()
        {
            foostepSoundManager.Play(foostepSoundManager.sounds[stepSoundCount++ % foostepSoundManager.sounds.Count]);
        }



    }
}
