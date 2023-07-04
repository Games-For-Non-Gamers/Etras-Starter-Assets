using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using UnityEngine;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_Jump : EtraAbilityBaseClass
    {
        [Header("Basics")]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.05f;
        private float _jumpTimeoutDelta;
        private bool alteredJumpInput;

        [Header("Cam Shake")]
        public bool jumpShakeEnabled = true;
        //The variables here are (intensity, time)
        public Vector2 jumpingShake = new Vector2(1f, 0.1f);


        //References
        private Animator _animator;
        private StarterAssetsInputs _input;
         bool _hasAnimator;
        private int _animIDJump;
        private int _animIDFreeFall;
        private GameObject _mainCamera;
        private AudioManager abilitySoundManager;

        public override void abilityStart()
        {
            //Get sfx manager
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = _mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
            //Set jump cooldown variable
            _jumpTimeoutDelta = JumpTimeout;
            //Get references
            mainController = GetComponentInParent<EtraCharacterMainController>();
            _input = GetComponentInParent<StarterAssetsInputs>();
            _hasAnimator = EtrasResourceGrabbingFunctions.TryGetComponentInChildren<Animator>(EtraCharacterMainController.Instance.modelParent);
            if (_hasAnimator) {
                _animator = EtraCharacterMainController.Instance.modelParent.GetComponentInChildren<Animator>();
                _animIDJump = Animator.StringToHash("Jump");
                _animIDFreeFall = Animator.StringToHash("FreeFall");
            }
        }

        [HideInInspector]
        public bool lockJump = false;
        public override void abilityUpdate()
        {
            alteredJumpInput = _input.jump;

            //If ability is disabled do not take jump input
            if (!abilityEnabled)
            {
                _input.jump = false;
                alteredJumpInput = false;
            }

            if (mainController.Grounded)
            {

                if (alteredJumpInput && _jumpTimeoutDelta <= 0.0f && lockJump == false)
                {
                    lockJump = true;
                    if (jumpShakeEnabled) { CinemachineShake.Instance.ShakeCamera(jumpingShake); }
                    
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    mainController._verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * mainController.Gravity);
                    // update animator if using character
                   

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                    else
                    {
                        abilitySoundManager.Play("Jump");
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {

                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // if we are not grounded, do not jump
                alteredJumpInput = false;
                _input.jump = false;
            }

        }
    }
}
