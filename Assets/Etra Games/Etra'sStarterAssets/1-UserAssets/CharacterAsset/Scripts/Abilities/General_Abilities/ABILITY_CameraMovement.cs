using Etra.StarterAssets.Input;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using static Etra.StarterAssets.Abilities.EtraAbilityBaseClass;
#endif

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_CameraMovement : EtraAbilityBaseClass
    {

        [Header("Basics")]
        //Camera sensitivity
        [Range(0,3)]
        public float mouseSensitivity = 1;
        [Range(0, 3)]
        public float joystickSensitivity = 1;
        public bool invertY;
        [HideInInspector] public float usedCameraSensitivity;
        [HideInInspector] public float aimSensitivity = 1;
        [SerializeField] public bool setForwardToPlayerLookDirection = true;
        private float rotateTowardsCharacterLookPointSpeed = 20f;

        //Layers to collide with
        [SerializeField] private LayerMask aimColliderMask = new LayerMask();
        private float distanceToTargetIfNoObjectIsHitByRay = 10;
        //Raycast feedback
        [HideInInspector] public bool objectHit;
        [HideInInspector] public Vector3 pointCharacterIsLookingAt;
        [HideInInspector] public RaycastHit raycastHit;

        //Debug
        [Header("Debug")]
        public bool camLookDebug;
        [SerializeField] private Transform debugTransform;

        //Cinemachine Cam Angle Limiting
        [Header("Camera Angle Limits")]
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -70.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;
        //Misc
        private const float _threshold = 0.01f;
        [HideInInspector] public float _cinemachineTargetYaw;
        [HideInInspector] public float _cinemachineTargetPitch;


        //X & Y Locks
        [Header("Locks")]
        public bool lockLookX = false;
        public bool lockLookY = false;
        private float camModX = 1;
        private float camModY = 1;

        //References 
        [HideInInspector]
        public GameObject playerCameraRoot;
        private StarterAssetsInputs _input;

        //SubAbilities
        private subAbilityUnlock[] _derivedSubAbilityUnlocks = new subAbilityUnlock[] {
                new subAbilityUnlock("UnlockX", true) ,
                new subAbilityUnlock("UnlockY", true) };

        public override subAbilityUnlock[] subAbilityUnlocks
        {
            get { return _derivedSubAbilityUnlocks ?? base.subAbilityUnlocks; }
            set { _derivedSubAbilityUnlocks = value; }
        }



#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
        private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";
#else
    private bool IsCurrentDeviceMouse = true;
#endif



        //Set defaults of gameplay variables 
        private void Reset()
        {
            setEtraMenuPlayerPrefs();
            if (this.gameObject.name == "Tempcube") { return; }
            if (GetComponentInParent<EtraCharacterMainController>().appliedGameplayType == EtraCharacterMainController.GameplayType.FirstPerson)
            {
                setForwardToPlayerLookDirection = true;
            }
            if (GetComponentInParent<EtraCharacterMainController>().appliedGameplayType == EtraCharacterMainController.GameplayType.ThirdPerson)
            {
                setForwardToPlayerLookDirection = false;
            }

            aimColliderMask = LayerMask.GetMask("Default");
            playerCameraRoot = GameObject.Find("EtraPlayerCameraRoot");
            OnValidate();
        }

        void setEtraMenuPlayerPrefs()
        {
            //LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
        }


        public override void abilityStart()
        {
            //Check OnValidate and x&y locks and set usedCameraSensitivity
            abilityCheckSubAbilityLocks();
            //Get references
            playerCameraRoot = GameObject.Find("EtraPlayerCameraRoot");
            _input = GetComponentInParent<StarterAssetsInputs>();
            playerCameraRoot.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponentInParent<PlayerInput>();
#endif
        }

        public override void abilityUpdate()
        {

            if (invertY)
            {
                camModY =  Mathf.Abs(camModY) *-1;
            }
            else
            {
                camModY = Mathf.Abs(camModY);
            }

            //Shoot a ray towards the center of the screen
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out raycastHit, 999f, aimColliderMask, QueryTriggerInteraction.Ignore))
            {
                //If the ray hits a layer that the ray can collide with, set the hit location as the pointCharacterIsLookingAt
                objectHit = true;
                pointCharacterIsLookingAt = raycastHit.point;

            }
            else
            {
                //If the ray hits nothing, select a point 'distanceToTargetIfNoObjectIsHitByRay' away to mark as pointCharacterIsLookingAt.
                objectHit = false;
                pointCharacterIsLookingAt = ray.GetPoint(distanceToTargetIfNoObjectIsHitByRay);

            }
            //Return if this ability is disabled
            if (!abilityEnabled)
            {
                return;
            }

            //If debug is active, teleport debugTransform to pointCharacterIsLookingAt 
            if (camLookDebug)
            {
                debugTransform.position = pointCharacterIsLookingAt;
            }

            //If the boolean is checked, set the players forward to where the player is facing
            if (setForwardToPlayerLookDirection)
            {
                var worldAimTarget = pointCharacterIsLookingAt;
                worldAimTarget.y = transform.position.y;
                var aimDirection = (worldAimTarget - transform.position).normalized;
                transform.parent.forward = Vector3.Lerp(transform.parent.forward, aimDirection, Time.deltaTime * rotateTowardsCharacterLookPointSpeed);
            }

        }


        public override void abilityLateUpdate()
        {
            //Return if this ability is disabled
            if (!abilityEnabled)
            {
                return;
            }

            updateUsedCameraSensitivity();

            //Set camera rotation from controller or mouse input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * camModX * usedCameraSensitivity * aimSensitivity * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * camModY * usedCameraSensitivity * aimSensitivity * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            playerCameraRoot.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);

        }


        public float updateUsedCameraSensitivity()
        {
            # if ENABLE_INPUT_SYSTEM
            if (_playerInput.currentControlScheme.Contains("KeyboardMouse"))
            {
                usedCameraSensitivity = mouseSensitivity;
                return mouseSensitivity;
            } else 
            {
                usedCameraSensitivity = joystickSensitivity;
                return joystickSensitivity;
            }
            #else
            usedCameraSensitivity = mouseSensitivity;
            return mouseSensitivity;
            #endif
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnValidate()
        {
            setEtraMenuPlayerPrefs();
            abilityCheckSubAbilityLocks();
        }

        private void abilityCheckSubAbilityLocks()
        {
            //Set the used variable to whatever the new camera sensitivity variable is.

            usedCameraSensitivity = mouseSensitivity;

            //If the locks are selected, then lock a certain direction
            if (lockLookX)
            {
                subAbilityUnlocks[0].subAbilityEnabled = false;
                camModX = 0;
            }
            else
            {
                subAbilityUnlocks[0].subAbilityEnabled = true;
                camModX = 1;
            }

            if (lockLookY)
            {
                subAbilityUnlocks[1].subAbilityEnabled = false;
                camModY = 0;
            }
            else
            {
                subAbilityUnlocks[1].subAbilityEnabled = true;
                camModY = 1;
            }
        }


        public override void abilityCheckSubAbilityUnlocks()
        {

            //If the locks are selected, then lock a certain direction
            if (subAbilityUnlocks[0].subAbilityEnabled == false)
            {
                lockLookX = true;
                camModX = 0;
            }
            else
            {
                lockLookX = false;
                camModX = 1;
            }

            if (subAbilityUnlocks[1].subAbilityEnabled == false)
            {
                lockLookY = true;
                camModY = 0;
            }
            else
            {
                lockLookY = false;
                camModY = 1;
            }
        }

    }
}