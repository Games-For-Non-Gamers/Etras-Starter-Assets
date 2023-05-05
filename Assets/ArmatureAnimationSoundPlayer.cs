using UnityEngine;

namespace EtrasStarterAssets
{
    public class ArmatureAnimationSoundPlayer : MonoBehaviour
    {
        private GameObject _mainCamera;
        private AudioManager foostepSoundManager;
        void Start()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            foostepSoundManager = _mainCamera.transform.Find("FootstepsAudio").GetComponent<AudioManager>();
        }

        int stepSoundCount = 0;
        public void PlayFootstep()
        {
            foostepSoundManager.Play(foostepSoundManager.sounds[stepSoundCount++ % foostepSoundManager.sounds.Count]);
        }

    }
}
