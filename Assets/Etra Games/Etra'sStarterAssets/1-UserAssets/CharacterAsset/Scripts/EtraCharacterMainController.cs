using Cinemachine;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Abilities.FirstPerson;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Interactables.Enemies;
using Etra.StarterAssets.Items;
using Etra.StarterAssets.Source;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using JetBrains.Annotations;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;
using UnityEngine.Networking.Types;
#endif

namespace Etra.StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class EtraCharacterMainController : MonoBehaviour
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //PUBLIC AND PRIVATE VARIABLES
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Instance (this is for easy referencing of the one Etra Character in the scene, Instance is set up in OnValidate())
        [HideInInspector] public static EtraCharacterMainController Instance;

        //************************
        //Gameplay type selector
        //************************
        //Make custom enums
        [HideInInspector]
        public enum GameplayType
        {
            FirstPerson,
            ThirdPerson
        }

        [System.Flags]
        public enum GameplayTypeFlags
        {
            All = 3,
            FirstPerson = 1,
            ThirdPerson = 2
        }

        [HideInInspector]
        public enum Model
        {
            DefaultArmature,
            Capsule,
            Voxel,
            None
        }
        //These public variables let you select what gameplay type you want
        [SerializeField] private GameplayType gameplayType;
        [SerializeField] private Model characterModel;
        [HideInInspector] public GameplayType appliedGameplayType = EtraCharacterMainController.GameplayType.FirstPerson;
        [HideInInspector] public Model appliedCharacterModel = EtraCharacterMainController.Model.Capsule;
        //This is space for the "Apply Gameplay Changes" button generated in UnityEditorForEtraCharacterMainController.cs
        [Space(50)]

        //************************
        //Gravity and floor collision variables
        //************************
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        [Tooltip("The maximum slope of an object the character can climb up")]
        public float maxWalkableSlope = 45;
        [Tooltip("If the player startes grounded")]
        public bool teleportToGroundAtStart = true;
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.2f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        [Header("Cam Shake")]
        public bool landingShakeEnabled = true;
        public Vector2 landingShake = new Vector2(1f, 0.1f);
        



        [HideInInspector]
        public float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        [HideInInspector]
        public float _fallTimeoutDelta;
        // animation IDs for floor collision events
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        [HideInInspector]
        public float currentSlopeAngle;//For slide ability in the future?

        //************************
        //Camera Setup variables
        //************************
        GameObject etraFollowCam;
        private NoiseSettings shake;
        private NoiseSettings handheldNormal;

        //************************
        //References
        //************************
        private bool _hasAnimator;
        private Animator _animator;
        private CharacterController _controller;
        private float characterHeight;
        private ABILITY_Jump abilityJump;
        private GameObject _mainCamera;
        private AudioManager abilitySoundManager;
        private StarterAssetsInputs inputs;

        //************************
        //Externally called function variables
        //************************
        private const int APPLIED_FORCES_AMOUNT = 6;
        private Vector3[] appliedForces = new Vector3[APPLIED_FORCES_AMOUNT]; //Up to six forces can be applied to the character in a single frame. Adjust this to change.


        //************************
        //Variables of child objects
        //************************
        [HideInInspector] public Transform modelParent;
        [HideInInspector] public StarterAssetsCanvas starterAssetCanvas;
        [HideInInspector] public EtraAbilityManager etraAbilityManager;
        [HideInInspector] public EtraFPSUsableItemManager etraFPSUsableItemManager;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //SETUP FUNCTIONS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Gameplay Type Changing Functions 

        //This function is what you want to run if you wish to change gameplay settings from an outside script
        public void applyGameplayChanges(GameplayType passedGameplayType, Model passedCharacterModel)
        {
            appliedGameplayType = passedGameplayType;
            appliedCharacterModel = passedCharacterModel;
            applyGameplayChanges();

        }

        //This function runs when the inspector button is pressed
        public void applyGameplayChangesInspectorButtonPressed()
        {
            appliedGameplayType = gameplayType;
            appliedCharacterModel = characterModel;
            applyGameplayChanges();
        }

        private void applyGameplayChanges()
        {
            //Update public inspector variables
            gameplayType = appliedGameplayType;
            characterModel = appliedCharacterModel;

            //Destroy the current Cinemachine Virtual Camera
            etraFollowCam = GameObject.Find("Etra'sStarterAssetsFollowCamera");
            DestroyImmediate(etraFollowCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>());
            //Create an editable Cinemachine Virtual Camera
            etraFollowCam.GetComponent<CinemachineVirtualCamera>().AddCinemachineComponent<Cinemachine3rdPersonFollow>();
            Cinemachine3rdPersonFollow newCamComponent = etraFollowCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            newCamComponent.VerticalArmLength = 0;
            //Maken the new camera collide with and ignore correct layers
            newCamComponent.CameraCollisionFilter = LayerMask.GetMask("Default");
            newCamComponent.IgnoreTag = "Player";
            newCamComponent.CameraRadius = 0.15f;

            switch (appliedGameplayType)
            {
                case EtraCharacterMainController.GameplayType.FirstPerson:
                    //FPS Default Camera Settings
                    newCamComponent.CameraDistance = 0;
                    newCamComponent.CameraSide = 0.6f;
                    newCamComponent.ShoulderOffset = new Vector3(0, 0, 0);
                    newCamComponent.Damping = new Vector3(0.0f, 0.0f, 0.0f);
                    etraFollowCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 75;

                    //FPS Ability Variable Default adjustment
                    if (GetComponentInChildren<ABILITY_CameraMovement>())
                    {
                        GetComponentInChildren<ABILITY_CameraMovement>().setForwardToPlayerLookDirection = true;
                    }
                    if (GetComponentInChildren<ABILITY_CharacterMovement>())
                    {
                        GetComponentInChildren<ABILITY_CharacterMovement>().rotateTowardMoveDirection = false;
                    }
                    break;

                case EtraCharacterMainController.GameplayType.ThirdPerson:
                    //TPS Default Camera Settings
                    newCamComponent.CameraDistance = 4;
                    newCamComponent.CameraSide = 1f;
                    newCamComponent.ShoulderOffset = new Vector3(0.7f, 0.25f, 0);
                    newCamComponent.Damping = new Vector3(0.1f, 0.25f, 0.3f);
                    etraFollowCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;

                    //TPS Ability Variable Default adjustment
                    if (GetComponentInChildren<ABILITY_CameraMovement>())
                    {
                        GetComponentInChildren<ABILITY_CameraMovement>().setForwardToPlayerLookDirection = false;
                    }
                    if (GetComponentInChildren<ABILITY_CharacterMovement>())
                    {
                        GetComponentInChildren<ABILITY_CharacterMovement>().rotateTowardMoveDirection = true;
                    }
                    break;
            }

            
            //Destroy the current character model
            foreach (Transform child in modelParent)
            {
                DestroyImmediate(child.gameObject);
            }
            GameObject model;
            //Select correct character model
            switch (appliedCharacterModel)
            {

                case EtraCharacterMainController.Model.DefaultArmature:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("DefaultArmatureCharacterModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("DefaultArmatureCharacterModel").transform.localPosition;
                    break;
                case EtraCharacterMainController.Model.Capsule:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("CapsuleCharacterModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("CapsuleCharacterModel").transform.localPosition;
                    break;
                case EtraCharacterMainController.Model.Voxel:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("VoxelCharacterModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("VoxelCharacterModel").transform.localPosition;
                    break;
                case EtraCharacterMainController.Model.None:
                    model = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("InvisibleCapsuleModel", modelParent, false, Vector3.zero);
                    model.transform.localPosition = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("InvisibleCapsuleModel").transform.localPosition;
                    break;
            }
        }

        #endregion
        #region Setup, Reset, and OnValidate functions
        private void Reset()
        {
            setChildObjects();
            GroundLayers = LayerMask.GetMask("Default");
        }

        public void setChildObjects()
        {
            modelParent = GetComponentInChildren<ModelParent>().gameObject.transform;
            starterAssetCanvas = GetComponentInChildren<StarterAssetsCanvas>();
            etraAbilityManager = GetComponentInChildren<EtraAbilityManager>();

            if (etraAbilityManager.GetComponent<ABILITY_Jump>() !=null)
            {
                abilityJump = etraAbilityManager.GetComponent<ABILITY_Jump>();
            }

            if (GetComponentInChildren<EtraFPSUsableItemManager>() != null)
            {
                etraFPSUsableItemManager = GetComponentInChildren<EtraFPSUsableItemManager>();
            }
        }

        private void setUpCinemachineScreenShakeNoiseProfile()
        {
            //Set up cinemachine screen shake profile
            if (Resources.Load<NoiseSettings>("6DShake") == null)
            {
                Debug.LogError("6DShake Cinemachine Noise Settings not found.");
                return;
            }
            shake = Resources.Load<NoiseSettings>("6DShake");


            if (Resources.Load<NoiseSettings>("HandheldNormal") == null)
            {
                Debug.LogError("HandheldNormal Cinemachine Noise Settings not found.");
                return;
            }
            handheldNormal = Resources.Load<NoiseSettings>("HandheldNormal");

            etraFollowCam = GameObject.Find("Etra'sStarterAssetsFollowCamera");
            etraFollowCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = shake;
        }

        private void AssignAnimationIDs()
        {
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
        }
        #endregion
        #region Grounded and Gravity Functions

        public RaycastHit hit;
        bool stuckOnWallSlope = false;
        Vector3 flatBeamToTarget;
        Vector3 lastStableStandingPosition;
        Collider colliderPlayerStuckOn;
        int hitAngle;
        Vector3 raycastDebugStart;
        Vector3 raycastDebugEnd;
        bool hasStableStand = false;



        public void GroundedCheck()
        {
            updateSlope();  //Get ground slope angle for slide ability if we add it

            //Make Overlap sphere at certain position
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Collider[] hitcolliders = new Collider[5];//max colliders to check
            //We are using Physics.OverlapSphereNonAlloc to save on peformance since we need to do annoying slide checks
            int hitColliderCount = Physics.OverlapSphereNonAlloc(spherePosition, GroundedRadius, hitcolliders, GroundLayers, QueryTriggerInteraction.Ignore);
            if (hitColliderCount > 0)
            {
                //This whole section exists to make sure the character is not grounded on any 46-89 degree surface. 
                //Without a special clause for this, the player can jump up objects with sharp angles.

                //This variable checks if any collider the player is on, can be stood on top of, or if it is too angled
                hasStableStand = false;

                for (int i = 0; i< hitColliderCount; i++)
                {
                    Vector3 target;
                    float controllerHeightMultiplier;
                    float lineLength;
                    if (hitcolliders[i] is MeshCollider meshCollider && !meshCollider.convex)
                    {
                        //If a non-convex mesh collider is hit, it is hard to get it's angle since you can't get its contact point.
                        //The next best thing we can go off of is the position of the object
                        target = hitcolliders[i].transform.position;
                        controllerHeightMultiplier = 0.05f;
                        lineLength = _controller.radius * 2;
                    }
                    else
                    {
                        //for all other colliders we can get the contact point of the object
                        target = hitcolliders[i].ClosestPoint(transform.position);
                        controllerHeightMultiplier = 0.05f;
                        lineLength = _controller.radius;
                    }
                   
                    //Make a flat raycast toward the target
                    flatBeamToTarget = new Vector3(target.x, 0, target.z)  - new Vector3(transform.position.x, 0, transform.position.z);
                    //The range of the raycast is only in the sphere collider where the character can start climbing walls.
                    //The beam starts from 5%  of player height

                    Vector3 raycastOrigin = transform.position + new Vector3(0, _controller.height * controllerHeightMultiplier, 0);
                    //This raycast checks if there is an object in between the bottom of the capsule collider from it's radius
                    //Basically if an object is in the less thick bottom of the collider
                    if (Physics.Raycast(raycastOrigin, flatBeamToTarget.normalized, out hit, lineLength, GroundLayers, QueryTriggerInteraction.Ignore))
                    {

                        //Check if the angle is larger than the max walkable slope
                        hitAngle = (int)Vector3.Angle(Vector3.up, hit.normal);
                        if (hitAngle > maxWalkableSlope)
                        {
                            hasStableStand = false;
                            stuckOnWallSlope = true;
                            colliderPlayerStuckOn = hitcolliders[i];

                        }
                        //If the angle is walkable, then apply grounded and break from the loop. There is no need to check the other colliders
                        else
                        {
                            hasStableStand = true;
                            break;
                        }

                        //Regardless of the angle stability collider, if colliding with a non-convex mesh collider, check if there is a floor under the player
                        if (hitcolliders[i] is MeshCollider meshCollider1 && !meshCollider1.convex)
                        {
                            if (!Physics.Raycast(raycastOrigin, Vector3.down, out hit, lineLength, GroundLayers, QueryTriggerInteraction.Ignore))
                            {
                                hasStableStand = false;
                            }
                        }

                        //DEBUG
                        /*
                        Debug.Log(flatBeamToTarget.normalized);
                        raycastDebugStart = raycastOrigin;
                        raycastDebugEnd = hit.point;
                        Debug.Log($"{hitAngle}");
                        */

                    }
                    else // if the Raycast is out of range, apply grounded and break from the loop.There is no need to check the other colliders
                    {
                        hasStableStand = true;
                        break;
                    }

                }
            }
            else //If the sphere is hitting no colliders then set grounded to false
            {
                stuckOnWallSlope = false; //They are not stuck on the wall if they are not touching anything
                hasStableStand = false;
            }

            //Make stuckOnWallSlope false if an earlier object homehow triggered this and was later corrected
            if (hasStableStand)
            {
                stuckOnWallSlope = false;
                Grounded = true;
            }
            else
            {
                Grounded = false;
            }

            if (!stuckOnWallSlope)
            {
                lastStableStandingPosition = this.transform.position;
                colliderPlayerStuckOn = null;
            }

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }


        void updateSlope()
        {
            RaycastHit slopeHit;
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, characterHeight * 0.5f + 0.3f))
            {
                currentSlopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            }
        }

        bool pauseGravityGain = false;
        bool jumpReset = true;
        Collider savedStuckCollider;
        private void ApplyGravity()
        {
            if (Grounded)
            {

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                //This jump reset code is to prevent repeatedly jumping in singular frames.
                if (jumpReset == true)
                {

                    if (abilityJump != null)
                    {
                        abilityJump.lockJump = false;
                    }
      
                    jumpReset = false;

                    //Only play sfx and animation if falling longer than the FallTimeout
                    if (_fallTimeoutDelta < 0.0f)
                    {
                        if (_hasAnimator)
                        {
                            _animator.SetBool(_animIDJump, false);
                        }
                        else
                        {
                                abilitySoundManager.Play("Land");
                        }

                        if (landingShakeEnabled) { CinemachineShake.Instance.ShakeCamera(landingShake); }
                    }

                }
                //Regardless of jump ability,  reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;
            }
            else
            {
                // Decrease the fall timeout till it triggers at >0
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }

                }
                //Reset the  jump ability
                jumpReset = true;
            }

            // Regardless of the player being grounded, apply gravity over time, and tilt with slopes 
            if (_verticalVelocity < _terminalVelocity)
            {
                if (!pauseGravityGain)
                {
                    _verticalVelocity += Gravity * Time.deltaTime;
                }
                
                if (!stuckOnWallSlope)
                {
                    pauseGravityGain = false;
                    addConstantForceToEtraCharacter(new Vector3(0, _verticalVelocity, 0));
                }
                else // player is on sloped wall or falling off an edge
                {
                    Vector3 angledForce = Vector3.ProjectOnPlane(flatBeamToTarget, hit.normal).normalized;
                    if (hitAngle != 90) // If the player is on a sloped wall
                    {

                        if (Physics.Raycast(transform.position, Vector3.down, out hit, characterHeight * 0.1f) && currentSlopeAngle > 1 && currentSlopeAngle < maxWalkableSlope)
                        {
                        }
                        else
                        {
                            savedStuckCollider = null;
                            addConstantForceToEtraCharacter(angledForce * _verticalVelocity);
                        }
                    }
                    else
                    {
                        savedStuckCollider = colliderPlayerStuckOn;

                        if (savedStuckCollider is MeshCollider meshCollider && !meshCollider.convex)
                        {
                            pauseGravityGain = true;//The is to prevent crazy grabity buildup in set clipping states
                            //If they are touching a non-convex mesh collider the angle is more difficult to figure, so we have to guess
                            //since we only know the mesh transform, and not mesh raycast point of contact
                            Vector3 towardLastStablePoint = new Vector3(lastStableStandingPosition.x, 0, lastStableStandingPosition.z) - new Vector3(transform.position.x, 0, transform.position.z);
                            addConstantForceToEtraCharacter((towardLastStablePoint.normalized) * -0.25f);
                            addConstantForceToEtraCharacter((flatBeamToTarget.normalized) * -0.25f);
                        }
                        else
                        {
                            //for all other colliders we can get the contact point of the object and simply add a force opposite of that
                            addConstantForceToEtraCharacter((flatBeamToTarget.normalized) * -1);
                        }
                        
                    }
                    
                }
                
            }

        }



        private void OnDrawGizmosSelected()
        {
            _controller = GetComponent<CharacterController>();
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
            Gizmos.DrawLine(raycastDebugStart, raycastDebugEnd);
        }

        #endregion

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //MAIN FUNCTIONS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void Awake()
        {
            //Set up Instance so it can be easily referenced. 
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Instance = this;
            }
            Physics.autoSyncTransforms = true;
            setChildObjects();
        }

        private void Start()
        {
            //Run setup functions
            _fallTimeoutDelta = FallTimeout;
            setUpCinemachineScreenShakeNoiseProfile();
            AssignAnimationIDs();
            //Set reference variables
            _hasAnimator = TryGetComponent(out _animator);
            _hasAnimator = EtrasResourceGrabbingFunctions.TryGetComponentInChildren<Animator>(modelParent);
            if (_hasAnimator) { _animator = modelParent.GetComponentInChildren<Animator>(); }
            _controller = GetComponent<CharacterController>();
            maxWalkableSlope = _controller.slopeLimit;
            characterHeight = _controller.height;
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = _mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
            inputs = GetComponent<StarterAssetsInputs>();
            teleportToGround();
        }


        public void teleportToGround()
        {
            Vector3 moveDown = new Vector3(0, -0.01f, 0);
            if (teleportToGroundAtStart)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f,  GroundLayers, QueryTriggerInteraction.Ignore))
                {
                    Grounded = false;
                    while (Grounded != true)
                    {
                        transform.position += moveDown;
                        //force a grounded check
                        GroundedCheck();
                    }
                }
                else
                {
                    Debug.LogWarning("There is no ground beneath the player, so they cannot: teleportToGroundAtStart.");
                }
            }
        }

        private void Update()
        {
            //Calculate Grounded and gravity
            GroundedCheck();
            ApplyGravity();
            //Apply vertical velocity from gravity or jump every frame
   

            
            updateImpulseVariables();

        }

        private void LateUpdate()
        {
            //Apply movement and forces to the character controller
            Vector3 overallForce = Vector3.zero;
            // string log= "";
            for (int i = 0; i < appliedForces.Length; i++)
            {
                //log += appliedForces[i] + ", ";
                overallForce += appliedForces[i];
            }
            //We want to call character controller .Move() once every frame with one overall vector or it acts wonky.  
            // Debug.Log(log);
            _controller.Move(overallForce * Time.deltaTime);
            //Reset all applied forces to Vector3.zero
            appliedForces = new Vector3[APPLIED_FORCES_AMOUNT];
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //COLLISIONS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        bool damageOnCollisionBool = false;
        int damageOnCollisionValue;
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (etraAbilityManager.GetComponent<ABILITY_RigidbodyPush>())
            {
                etraAbilityManager.GetComponent<ABILITY_RigidbodyPush>().PushRigidBodies(hit);
            }

            if (damageOnCollisionBool)
            {
                IDamageable<int> isDamageableCheck = hit.gameObject.GetComponent<IDamageable<int>>();
                if (isDamageableCheck != null)
                {
                    isDamageableCheck.TakeDamage(damageOnCollisionValue);
                    damageOnCollisionBool = false;
                }
            }

        }



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //CUSTOM FUNCTIONS TO BE CALLED FROM OTHER SCRIPTS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void addConstantForceToEtraCharacter(Vector3 addedForce)
        {
            int emptyIndex = -1;
            for (int i = 0; i < appliedForces.Length; i++)
            {
                if (appliedForces[i].Equals(Vector3.zero))
                {
                    emptyIndex = i;
                    i = appliedForces.Length;
                }

                if (i == appliedForces.Length - 1)
                {
                    Debug.Log("Cannot add any more forces to character controller in this singular frame. To add more forces to the character" +
                        " please increase APPLIED_FORCES_AMOUNT in EtraCharacterMainController.cs .");
                    return;
                }

            }
            appliedForces[emptyIndex] = addedForce;
        }


        Vector3 impact = Vector3.zero;
        public void updateImpulseVariables()
        {
            if (impact.magnitude > 0.2f)
            {
                _controller.Move(impact * Time.deltaTime);

            }

            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
        }

        public void addImpulseForceToEtraCharacter(Vector3 direction, float force)
        {
            direction.Normalize();
            if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
            impact += direction.normalized * force / 3.0f;
        }


        public void addImpulseForceWithDamageToEtraCharacter(Vector3 direction, float force, int damage, float cooldown)
        {
            direction.Normalize();
            if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
            impact += direction.normalized * force / 3.0f;

            damageOnCollisionValue = damage;
            damageOnCollisionBool = true;
            StartCoroutine(damageOnCollisionCooldown(cooldown));

        }

        IEnumerator damageOnCollisionCooldown(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);
            damageOnCollisionBool = false;
        }


        public void disableAllActiveAbilities()
        {
            etraAbilityManager.disableAllActiveAbilities();
            if (etraFPSUsableItemManager != null)
            {
                etraFPSUsableItemManager.disableFPSItemInputs();
            }
        }

        public void disableAllActiveAbilitiesAndSubAblities()
        {
            disableAllActiveAbilities();
            foreach (EtraAbilityBaseClass ability in etraAbilityManager.characterAbilityUpdateOrder)
            {
                for (int i = 0; i < ability.subAbilityUnlocks.Length; i++)
                {
                    ability.subAbilityUnlocks[i].subAbilityEnabled = false;
                }
                ability.abilityCheckSubAbilityUnlocks();
            }
        }

        public void enableAllActiveAbilities()
        {
            etraAbilityManager.enableAllActiveAbilities();
            if (etraFPSUsableItemManager != null)
            {
                etraFPSUsableItemManager.enableFPSItemInputs();
            }
            
        }

        public void enableAllActiveAbilitiesAndSubAblities()
        {
            enableAllActiveAbilities();
            foreach (EtraAbilityBaseClass ability in etraAbilityManager.characterAbilityUpdateOrder)
            {

                    for (int i = 0; i < ability.subAbilityUnlocks.Length; i++)
                    {
                        ability.subAbilityUnlocks[i].subAbilityEnabled = true;
                    }
                    ability.abilityCheckSubAbilityUnlocks();
            }

        }

        public EtraCameraSettings getCameraSettings()
        {
            etraFollowCam = GameObject.Find("Etra'sStarterAssetsFollowCamera");
            CinemachineVirtualCamera camComponent = etraFollowCam.GetComponent<CinemachineVirtualCamera>();
            EtraCameraSettings settings = new EtraCameraSettings(camComponent);
            return settings;
        }

        public void setCameraSettings(EtraCameraSettings camSettings)
        {
            etraFollowCam = GameObject.Find("Etra'sStarterAssetsFollowCamera");
            CinemachineVirtualCamera camComponent = etraFollowCam.GetComponent<CinemachineVirtualCamera>();
            camSettings.applySettingsToCam(camComponent);
        }

        public void setCameraSettingsOverTime(EtraCameraSettings camSettings, float time)
        {
            etraFollowCam = GameObject.Find("Etra'sStarterAssetsFollowCamera");
            CinemachineVirtualCamera camComponent = etraFollowCam.GetComponent<CinemachineVirtualCamera>();
            camSettings.applySettingsToCamOverTime(camComponent, time);
        }


        public class EtraCameraSettings
        {
            public float fov;
            public float cameraDistance;
            public float cameraSide;
            public Vector3 shoulderOffset;
            public Vector3 damping;
            public EtraCameraSettings(CinemachineVirtualCamera cam)
            {
                fov = cam.m_Lens.FieldOfView;
                Cinemachine3rdPersonFollow thirdPerson = cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                cameraDistance = thirdPerson.CameraDistance;
                cameraSide = thirdPerson.CameraSide;
                shoulderOffset = thirdPerson.ShoulderOffset;
                damping = thirdPerson.Damping;
            }

            public EtraCameraSettings(float fov, float cameraDistance, float cameraSide, Vector3 shoulderOffset, Vector3 damping)
            {
                this.fov = fov;
                this.cameraDistance = cameraDistance;
                this.cameraSide = cameraSide;
                this.shoulderOffset = shoulderOffset;
                this.damping = damping;
            }


            public void applySettingsToCam(CinemachineVirtualCamera cam)
            {
                cam.m_Lens.FieldOfView = fov;
                Cinemachine3rdPersonFollow thirdPerson = cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                thirdPerson.CameraDistance = cameraDistance;
                thirdPerson.CameraSide = cameraSide;
                thirdPerson.ShoulderOffset = shoulderOffset;
                thirdPerson.Damping = damping;
            }

            public void applySettingsToCamOverTime(CinemachineVirtualCamera cam, float time)
            {
                EtraCameraSettings currentCamSettings = new EtraCameraSettings(cam);
                LeanTween.value(cam.gameObject, currentCamSettings.fov, fov, time).setOnUpdate((float fovValue) => { cam.m_Lens.FieldOfView = fovValue; }).setEaseInOutSine();
                Cinemachine3rdPersonFollow thirdPerson = cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                LeanTween.value(cam.gameObject, currentCamSettings.cameraDistance, cameraDistance, time).setOnUpdate((float cameraDistance) => { thirdPerson.CameraDistance = cameraDistance; ; }).setEaseInOutSine();
                LeanTween.value(cam.gameObject, currentCamSettings.cameraSide, cameraSide, time / 3).setOnUpdate((float cameraSide) => { thirdPerson.CameraSide = cameraSide;  }).setEaseInOutSine();
                LeanTween.value(cam.gameObject, currentCamSettings.shoulderOffset, shoulderOffset, time/3).setOnUpdate((Vector3 shoulderOffset) => { thirdPerson.ShoulderOffset = shoulderOffset;  }).setEaseInOutSine();
                LeanTween.value(cam.gameObject, currentCamSettings.damping, damping, time/3).setOnUpdate((Vector3 damping) => { thirdPerson.Damping = damping; }).setEaseInOutSine();
            }

        }
    }

}

