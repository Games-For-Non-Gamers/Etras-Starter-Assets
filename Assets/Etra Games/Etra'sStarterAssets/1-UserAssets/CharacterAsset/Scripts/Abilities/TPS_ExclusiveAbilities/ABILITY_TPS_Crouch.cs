using System.Collections.Generic;
using Cinemachine;
using Etra.StarterAssets.Abilities.FirstPerson;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source;
using UnityEngine;
using UnityEngine.Events;

namespace Etra.StarterAssets.Abilities.ThirdPerson
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.ThirdPerson)]

    public class ABILITY_TPS_Crouch : EtraAbilityBaseClass
    {
        [HideInInspector]
        public enum crouchType
        {
            Toggle,
            Hold
        }

        [Header("Basics")] [SerializeField] private crouchType CrouchType = crouchType.Hold;
        [SerializeField] private float crouchedMovementSpeed = 1.5f;
        [SerializeField] private float crouchCameraHeight = 0.5f;
        [SerializeField]
        [Tooltip("Prohibits to Dash and Jump during crouching")]
        private bool blockDashAndJumpAbilities = true;
        [SerializeField]
        [Tooltip("When Crouching, these abilities will be disabled.")]
        private List<EtraAbilityBaseClass> abilitiesToBlockWhileCrouching = new List<EtraAbilityBaseClass>();

        [Header("Stay Crouched on Obstacle Detection")] [SerializeField]
        private bool changeCameraViewOnObstacleDetection = true;

        [SerializeField] private Vector2 cameraClampOnCantStandup = new Vector2(5, -5);
        
        [Tooltip("This camera will be activated when the player is crouched and can't stand up due to an obstacle. Standard is Etra'sStarterAssetsThirdPersonAimCamera. Will be overriden with the value set in the Inspector.")]
        [SerializeField] private CinemachineVirtualCamera occlusionCamera;

        [Header("Crouchable Obstacle Detection")] [SerializeField]
        private bool autoCrouchDetection = true;

        [SerializeField] private Vector3 autoCrouchRaycastOriginOffset = new Vector3(0,0.3f,0.5f);

        // Layer Masks
        [SerializeField] private  LayerMask obstacleMask = 1 << 0 ;
        [SerializeField] private  LayerMask autoCrouchDetectionMask = 1 << 0 ;

        [Header("Unity Events")]
        public UnityEvent OnCrouch;
        public UnityEvent OnStandUp;
        public UnityEvent OnChangeState;

        // Bools
        private bool m_IsCrouching;
        private bool m_PrevCrouchState;

        // Floats
        private float m_DefaultStandingHeight;
        private float m_CrouchHeight = 0.5f;
        private float m_DefaultCameraHeight;

        // Others Components
        private StarterAssetsInputs m_StarterAssetsInputs;
        private CharacterController m_CharacterController;
        private ABILITY_CharacterMovement m_MovementAbility;
        private Animator m_Animator;
        private ABILITY_CameraMovement m_CameraMovement;

        // Vectors
        private Vector3 m_DefaultStandingCenter;
        private Vector3 m_CrouchCenter;
        private Vector2 m_DefaultCameraClampValues;
        
        // Transforms
        private Transform m_CameraRoot;
        private GameObject _autoCrouchRaycastOriginTemp;

        // Static
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
        private bool _debug;

        public override void abilityStart()
        {
            // Initializations
            m_StarterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            m_CharacterController = GetComponentInParent<CharacterController>();
            m_MovementAbility = GetComponent<ABILITY_CharacterMovement>();
            m_Animator = EtraCharacterMainController.Instance.modelParent.GetComponentInChildren<Animator>();
            m_CameraMovement = GetComponent<ABILITY_CameraMovement>();
            m_CameraRoot = m_CameraMovement.playerCameraRoot.transform;

            InitializeParentedRaycastOrigin();

            m_DefaultStandingHeight = m_CharacterController.height;
            m_CrouchHeight = m_DefaultStandingHeight / 2;
            m_DefaultStandingCenter = m_CharacterController.center;
            m_CrouchCenter = m_DefaultStandingCenter / 2;
            m_MovementAbility.crouchSpeed = crouchedMovementSpeed;
            
            //Camera Setup
            m_DefaultCameraClampValues = new Vector2(m_CameraMovement.TopClamp, m_CameraMovement.BottomClamp);
            m_DefaultCameraHeight = m_CameraRoot.localPosition.y;
            
            if(occlusionCamera == null) occlusionCamera = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("Etra'sStarterAssetsThirdPersonAimCamera", GameObject.FindGameObjectWithTag("Player").transform.parent).GetComponent<CinemachineVirtualCamera>();
            occlusionCamera.Follow = m_CameraRoot;
        }

        private void InitializeParentedRaycastOrigin()
        {
            _autoCrouchRaycastOriginTemp = new GameObject("autoCrouchRaycastOriginTemp");
            _autoCrouchRaycastOriginTemp.transform.parent = transform;
            _autoCrouchRaycastOriginTemp.transform.localPosition = autoCrouchRaycastOriginOffset;
        }

        public override void abilityUpdate()
        {
            if (!abilityEnabled) return;

            switch (CrouchType)
            {
                case crouchType.Hold:
                    HandleHoldToCrouch();
                    break;
                case crouchType.Toggle:
                    HandleToggleCrouch();
                    break;
            }

            if (changeCameraViewOnObstacleDetection) occlusionCamera.gameObject.SetActive(m_IsCrouching && !CanStandUp());
            if (autoCrouchDetection && DetectCrouchableObstacle() && !m_IsCrouching) Crouch();
            if (!CanStandUp() && m_IsCrouching)
            {
                m_CameraMovement.TopClamp = cameraClampOnCantStandup.x;
                m_CameraMovement.BottomClamp = cameraClampOnCantStandup.y;
            }
            else
            {
                m_CameraMovement.TopClamp = m_DefaultCameraClampValues.x;
                m_CameraMovement.BottomClamp = m_DefaultCameraClampValues.y;
            }
        }

        private bool DetectCrouchableObstacle()
        {
            if (m_IsCrouching) return false;
            var rayCastOrigin = _autoCrouchRaycastOriginTemp.transform.position;
            var rayCastOriginlocalPosition = _autoCrouchRaycastOriginTemp.transform.localPosition;
            Vector3 boxHalfExtents = m_CharacterController.bounds.extents/2f;
            
            var isAboveStandingHeight = Physics.Raycast(rayCastOrigin, Vector3.up,
                m_DefaultStandingHeight - rayCastOriginlocalPosition.y, autoCrouchDetectionMask);

            var isAboveCrouchHeight = Physics.CheckBox(rayCastOrigin + Vector3.up * ((m_CrouchHeight - rayCastOriginlocalPosition.y) / 2f),
                boxHalfExtents, Quaternion.identity, autoCrouchDetectionMask);
            
            return isAboveStandingHeight && !isAboveCrouchHeight;
        }

        private bool CanStandUp()
        {
            return !Physics.BoxCast(transform.position, m_CharacterController.bounds.extents, Vector3.up,
                transform.rotation, m_DefaultStandingHeight, obstacleMask);
        }

        private void HandleToggleCrouch()
        {
            if (m_StarterAssetsInputs.crouch && !m_PrevCrouchState)
            {
                m_PrevCrouchState = true;
                if (m_IsCrouching)
                {
                    StandUp();
                }
                else
                {
                    Crouch();
                }
            }

            if (!m_StarterAssetsInputs.crouch) m_PrevCrouchState = false;
        }

        private void HandleHoldToCrouch()
        {
            if (m_StarterAssetsInputs.crouch)
            {
                Crouch();
            }
            else if (CanStandUp())
            {
                StandUp();
            }
        }

        private void Crouch()
        {
            m_IsCrouching = true;
            OnCrouch.Invoke();
            OnChangeState.Invoke();

            m_Animator.SetBool(IsCrouching, true);
            m_MovementAbility.isCrouched = true;

            m_CharacterController.height = m_CrouchHeight;
            m_CharacterController.center = m_CrouchCenter;
            
            ChangeCameraRoot(crouchCameraHeight);
            BlockAbilities(true);
            
        }

        private void StandUp()
        {
            if (!CanStandUp()) return;
            m_IsCrouching = false;
            OnStandUp.Invoke();
            OnChangeState.Invoke();

            m_Animator.SetBool(IsCrouching, false);

            m_MovementAbility.isCrouched = false;

            m_CharacterController.height = m_DefaultStandingHeight;
            m_CharacterController.center = m_DefaultStandingCenter;
            
            m_CameraMovement.TopClamp = m_DefaultCameraClampValues.x;
            m_CameraMovement.BottomClamp = m_DefaultCameraClampValues.y;
            ChangeCameraRoot(m_DefaultCameraHeight);
            BlockAbilities(false);
        }
        
        private void ChangeCameraRoot(float newHeight)
        {
            var camLocalPos =  m_CameraRoot.localPosition;
            
                camLocalPos = new Vector3(camLocalPos.x, newHeight, camLocalPos.z);
                m_CameraRoot.localPosition = camLocalPos;
        }

        private void OnDrawGizmos()
        {
            if (!_debug) return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.localPosition + autoCrouchRaycastOriginOffset, m_CrouchHeight/2);
        }
        
        private void OnValidate()
        {
            if (blockDashAndJumpAbilities)
            {
                ABILITY_Jump jumpComponent;
                if (TryGetComponent(out jumpComponent))
                {
                    if(!abilitiesToBlockWhileCrouching.Contains(jumpComponent))
                        abilitiesToBlockWhileCrouching.Add(jumpComponent);
                }
  
                ABILITY_Dash dashComponent;
                if (TryGetComponent(out dashComponent))
                {
                    if(!abilitiesToBlockWhileCrouching.Contains(dashComponent))
                        abilitiesToBlockWhileCrouching.Add(dashComponent);
                }
            }
            else
            {
                ABILITY_Jump jumpComponent;
                if (TryGetComponent(out jumpComponent))
                {
                    if(abilitiesToBlockWhileCrouching.Contains(jumpComponent))
                        abilitiesToBlockWhileCrouching.Remove(jumpComponent);
                }
  
                ABILITY_Dash dashComponent;
                if (TryGetComponent(out dashComponent))
                {
                    if(abilitiesToBlockWhileCrouching.Contains(dashComponent))
                        abilitiesToBlockWhileCrouching.Remove(dashComponent);
                }
            }
        }
 
        private void BlockAbilities(bool b)
        {
            foreach (var ability in abilitiesToBlockWhileCrouching)
            {
                ability.abilityEnabled = !b;
            }
        }
        
    }
}
