using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using UnityEngine;
using static Etra.StarterAssets.EtraCharacterMainController;

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
        public bool jumpInput;

        [Header("Cam Shake")]
        public bool jumpShakeEnabled = true;
        //The variables here are (intensity, time)
        public Vector2 jumpingShake = new Vector2(1f, 0.1f);

        //References
        private Animator _animator;
        private StarterAssetsInputs _input;
        [HideInInspector] public bool _hasAnimator; 
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

        public void setAnimator(bool enabled)
        {
            if (enabled)
            {
                _hasAnimator = EtrasResourceGrabbingFunctions.TryGetComponentInChildren<Animator>(FindObjectOfType<EtraCharacterMainController>().modelParent);
                if (_hasAnimator)
                {
                    _animator = FindObjectOfType<EtraCharacterMainController>().modelParent.GetComponentInChildren<Animator>();
                    _animIDJump = Animator.StringToHash("Jump");
                    _animIDFreeFall = Animator.StringToHash("FreeFall");
                }
            }
            else
            {
                _hasAnimator = false;
            }


        }
   

        [HideInInspector]
        public bool lockJump = false;
        public override void abilityUpdate()
        {
            jumpInput = _input.jump;

            //If ability is disabled do not take jump input
            if (!abilityEnabled)
            {
                _input.jump = false;
                jumpInput = false;
            }

            if (mainController.Grounded  && mainController.gravityActive)
            {

                if (jumpInput && _jumpTimeoutDelta <= 0.0f )
                {
                        jump();
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }

            }
            else if (!mainController.gravityActive)
            {
                //Keep the jump input at its value to be used by other scripts if gravity is being messed with
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (mainController._fallTimeoutDelta >= 0.0f)
                {
                    mainController._fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }


                // if we are not grounded, do not jump
                jumpInput = false;
                _input.jump = false;
            }

        }

        public void jump()
        {
            jump(JumpHeight);
        }

        public void jump(float height)
        {
            if (lockJump)
            {
                return;
            }
            if (jumpShakeEnabled) { CinemachineShake.Instance.ShakeCamera(jumpingShake); }
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            mainController._verticalVelocity = Mathf.Sqrt(height * -2f * mainController.Gravity);
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

        public void jumpLaunch(Vector3 direction, float force)
        {
            jumpLaunch(direction, force, true);
        }

        public void jumpLaunch(Vector3 direction, float force, bool playEffects)
        {
            if (lockJump)
            {
                return;
            }
            lockJump = true;

            if (playEffects)
            {
                if (jumpShakeEnabled) { CinemachineShake.Instance.ShakeCamera(jumpingShake); }
                if (_hasAnimator)
                {
                    Debug.Log("e");
                    _animator.SetBool(_animIDJump, true);
                }
                else
                {
                    abilitySoundManager.Play("Jump");
                }
            }

            EtraCharacterMainController.Instance.addImpulseForceToEtraCharacter(direction, force);
        }


    }
}
