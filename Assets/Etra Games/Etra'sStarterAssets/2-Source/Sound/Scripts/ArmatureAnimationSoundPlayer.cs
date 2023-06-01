using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using UnityEngine;

namespace EtrasStarterAssets
{
    public class ArmatureAnimationSoundPlayer : MonoBehaviour
    {
        private GameObject _mainCamera;
        private AudioManager abilitySoundManager;
        private AudioManager foostepSoundManager;
        private ABILITY_CharacterMovement _characterMovement;
        void Start()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = _mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
            foostepSoundManager = _mainCamera.transform.Find("FootstepsAudio").GetComponent<AudioManager>();

            if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CharacterMovement>() != null)
            {
                _characterMovement = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CharacterMovement>();
            }
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


        public void PlayFootstepAnimatedWalk()
        {
            //Only play if player is actually moving at a sprint speed. This is to deal with the randomness of the default animator belnd tree on idle.
            if (_characterMovement.passedMovementInput != Vector2.zero && _characterMovement.moveSpeed > 0.1f && _characterMovement.moveSpeed < 5.5f)
            {
                foostepSoundManager.Play(foostepSoundManager.sounds[stepSoundCount++ % foostepSoundManager.sounds.Count]);
            }
        }

        public void PlayFootstepAnimatedRun()
        {
            //Only play if player is actually moving at a sprint speed. This is to deal with the randomness of the default animator belnd tree on idle.
            if (_characterMovement.passedMovementInput != Vector2.zero && _characterMovement.moveSpeed > 5.5f)
            {
                foostepSoundManager.Play(foostepSoundManager.sounds[stepSoundCount++ % foostepSoundManager.sounds.Count]);
            }

        }



    }
}
