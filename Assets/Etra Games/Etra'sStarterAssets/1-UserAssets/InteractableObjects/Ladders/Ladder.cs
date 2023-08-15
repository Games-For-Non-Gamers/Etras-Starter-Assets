using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source.Interactions;
using EtrasStarterAssets;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

namespace Etra.StarterAssets.Interactables
{
    public class Ladder : MonoBehaviour
    {

        /* ladder tuern and lock
             case AnimationEvents.UnlockPlayer:
                 etraCharacterMainController.enableAllActiveAbilities();
                 break;

             case AnimationEvents.LockPlayer:
                 etraCharacterMainController.disableAllActiveAbilities();
                 break;
  * 
  */



        public enum LadderAttach
        {
            AutoAttach,
            InteractAttach
        }

        public enum JumpOffLadder
        {
            JumpAwayFromLadder,
            NoJumpOffLadder
        }

        public enum JumpAtEnd
        {
            JumpAtEnd,
            NoJumpAtEnd
        }

        public enum LadderMovement
        {
            FreeMove,
            AnimateAndLock ///tween and rotate main char and cam? Lock movement and jump abilities?
        }

        [Header ("Basic")]
        public float ladderWalkClimbSpeed = 250;
        public float ladderSprintClimbSpeed = 500;
        public float jumpAwayFromLadderForce = 70;
        public bool canClimbLadder = true;
        //public bool jumpAtEnd = true;
        public LadderAttach ladderAttachType = LadderAttach.AutoAttach;
        public JumpOffLadder jumpOffLadder = JumpOffLadder.JumpAwayFromLadder;
        public JumpAtEnd jumpAtEnd = JumpAtEnd.JumpAtEnd;
        public LadderMovement ladderMovement = LadderMovement.FreeMove;

        float stepTime = 0;
        bool isClimbingLadder = false;
        bool attatchedToLadder = false;

        [Header("Events")]
        public UnityEvent ladderAttach;
        public UnityEvent ladderDetatch;
        public UnityEvent ladderTopReached;

        [Header("References")]
        public AudioManager standardAudioManager;
        public AudioManager footstepAudioManager;
        public ObjectInteraction ladderBodyInteraction;
        public ObjectInteraction ladderTopInteraction;

        EtraCharacterMainController mainController;
        ABILITY_CharacterMovement movementAbility;
        ABILITY_Jump jumpAbility;
        LadderTopTrigger ladderTopTrigger;
        // Start is called before the first frame update



        void Start()
        {
            ladderBodyInteraction.isInteractable = false;
            ladderTopInteraction.isInteractable = false;
            mainController = FindObjectOfType<EtraCharacterMainController>();
            movementAbility = mainController.etraAbilityManager.GetComponent<ABILITY_CharacterMovement>(); //Add checks/warnings for the abilities
            jumpAbility = mainController.etraAbilityManager.GetComponent<ABILITY_Jump>();
            ladderTopTrigger =GetComponentInChildren<LadderTopTrigger>();
        }



        LayerMask allLayersMask = ~0;
        // Update is called once per frame
        bool playerInHitbox = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInHitbox = true;
                if (ladderAttachType == LadderAttach.AutoAttach)
                {
                    attachToLadder();
                }
            }


        }

 
        public void attachToLadder()
        {
            switch (ladderMovement)
            {
                case LadderMovement.FreeMove:
                    //Didn't really detail this patch much so it might be buggy, feel free to though yourself!
                    mainController.gravityActive = false;
                    jumpAbility.lockJump = false;
                    standardAudioManager.Play("LadderAttach");
                    attatchedToLadder = true;
                    ladderAttach.Invoke();

                    break;
                case LadderMovement.AnimateAndLock:

                    break;
            }
            

        }


        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInHitbox = true;

                switch (ladderAttachType)
                {
                    case LadderAttach.InteractAttach:
                        ladderBodyInteraction.isInteractable = true;
                        ladderTopInteraction.isInteractable = true;
                        break;
                }


                if (!attatchedToLadder)
                {
                    return;
                }
            }


            if (other.gameObject.CompareTag("Player") && canClimbLadder)
            {
                mainController.gravityActive = false;

                if (jumpOffLadder == JumpOffLadder.JumpAwayFromLadder && jumpAbility.jumpInput)
                {
                    jumpAbility.jumpLaunch(this.transform.forward, jumpAwayFromLadderForce, false);
                    return;
                }

                RaycastHit hit;

                float ladderClimbSpeed = 0;
                if (movementAbility.isSprinting)
                {
                    ladderClimbSpeed = ladderSprintClimbSpeed;
                }
                else
                {
                    ladderClimbSpeed = ladderWalkClimbSpeed;

                }

                isClimbingLadder = false;

                //If moving at ladder
                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0,0.2f,0), movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) 
                    && hit.collider.gameObject == this.gameObject 
                    && movementAbility.passedMovementInput != Vector2.zero)
                {
                    mainController.addConstantForceToEtraCharacter(Vector3.up * ladderClimbSpeed * Time.deltaTime);
                    isClimbingLadder = true;
                }

                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0, 0.2f, 0), -movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) 
                    && hit.collider.gameObject == this.gameObject
                    && movementAbility.passedMovementInput != Vector2.zero
                    && mainController.Grounded == false)
                {
                    movementAbility.preventMovement= true;
                    mainController.addConstantForceToEtraCharacter(Vector3.down * ladderClimbSpeed * Time.deltaTime);
                    isClimbingLadder = true;
                }
                else
                {
                    movementAbility.preventMovement = false;
                }

                if (isClimbingLadder)
                {
                    //moving on ladder if statement
                    float nextStepThreshold;
                    if (movementAbility.isSprinting)
                    {
                        nextStepThreshold = 50f/ ladderSprintClimbSpeed;
                    }
                    else
                    {
                        nextStepThreshold = 50f / ladderWalkClimbSpeed;
                    }

                    stepTime += Time.deltaTime;
                    if (stepTime > nextStepThreshold)
                    {
                        PlayLadderSteps();
                        stepTime = 0.0f;
                    }
                }
                else
                {
                    stepTime = 0.0f;
                }

            }

        }
        int stepSoundCount = 0;
        public void PlayLadderSteps()
        {
            footstepAudioManager.Play(footstepAudioManager.sounds[stepSoundCount++ % footstepAudioManager.sounds.Count]);
        }

    private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                attatchedToLadder = false;
                playerInHitbox = false;
                ladderBodyInteraction.isInteractable = false;
                ladderTopInteraction.isInteractable = false;
                mainController.gravityActive = true;
                movementAbility.preventMovement = false;
                ladderDetatch.Invoke();
                if (ladderTopTrigger.isOverlappingPlayer && jumpAtEnd == JumpAtEnd.JumpAtEnd)
                {
                    jumpAbility.jump();                    
                }
            }
        }


    }
}
/*
 * 
 * using Cinemachine;
using Etra.StandardMenus;
using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;
using static Etra.StarterAssets.EtraCharacterMainController;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class OpeningScene : MonoBehaviour
    {
        [Header("Basic")]
        public bool skipOpeningScene;
        [Header("Advanced")]
        public bool startCamAtStartPos = true;
        [Header("References")]
        public GameObject openingSceneUi;
        public GameObject nonGamerTutorialUi;

        //Rect transforms
        private AbilityOrItemPickup[] pickups;
        private AnimationTriggerPickup[] animPickups;

        //References
        private Star star;
        private GameObject playerSpawn;
        private GameObject camRoot;
        private GameObject cursorCanvas;
        private LevelController levelController;
        private EtraCharacterMainController character;

        //Wait times for cutscene
        private WaitForSecondsRealtime wait4p5Seconds = new WaitForSecondsRealtime(4.5f);
        private WaitForSecondsRealtime wait4Seconds = new WaitForSecondsRealtime(4f);
        private WaitForSecondsRealtime wait0p4Seconds = new WaitForSecondsRealtime(0.4f);
        private WaitForSecondsRealtime wait3p2Seconds = new WaitForSecondsRealtime(3.2f);
        private WaitForSecondsRealtime wait3Seconds = new WaitForSecondsRealtime(3f);
        private WaitForSecondsRealtime wait2Seconds = new WaitForSecondsRealtime(2f);
        private WaitForSecondsRealtime wait1Second = new WaitForSecondsRealtime(1f);

        //In code variables
        private EtraCameraSettings savedCameraSettings;
        private GameObject scoutStar;
        private GameObject scoutSpawn;

        private void Awake()
        {
            GetReferenceVariables();
            if (character.etraFPSUsableItemManager)
            {
                character.etraFPSUsableItemManager.weaponInitHandledElsewhere = true;
            }

            cursorCanvas.SetActive(false);
            pickups = levelController.chunks[levelController.chunks.Count - 1].gameObject.GetComponentsInChildren<AbilityOrItemPickup>();
            animPickups = levelController.chunks[levelController.chunks.Count - 1].gameObject.GetComponentsInChildren<AnimationTriggerPickup>();
            foreach (AbilityOrItemPickup a in pickups)
            {
                a.gameObject.SetActive(false);
            }
            foreach (AnimationTriggerPickup a in animPickups)
            {
                a.gameObject.SetActive(false);
            }
            nonGamerTutorialUi.gameObject.SetActive(false);
        }

        private void Start()
        {
            EtraStandardMenusManager menusManager = FindObjectOfType<EtraStandardMenusManager>();
            menusManager.canFreeze = false;

            character.disableAllActiveAbilities();
            savedCameraSettings = character.getCameraSettings();
            if (skipOpeningScene)
            {
                return;
            }
            if (startCamAtStartPos)
            {
                camRoot.transform.position = star.transform.position + new Vector3(0, 50, -30); //Start
            }


            //float fov, float cameraDistance, float cameraSide, Vector3 shoulderOffset, Vector3 damping
            EtraCameraSettings tempCamSettings = new EtraCameraSettings(60f, 0f, 0.6f, Vector3.zero, Vector3.zero);
            character.setCameraSettings(tempCamSettings);

            //Camera settings save 

            scoutStar = new GameObject("ScoutStar");
            scoutStar.transform.parent = star.transform;
            scoutStar.transform.localPosition = Vector3.zero;
            scoutSpawn = new GameObject("ScoutSpawn");
            scoutSpawn.transform.parent = playerSpawn.transform;
            scoutSpawn.transform.localPosition = new Vector3(0, 9, 0);
            scoutSpawn.AddComponent<BoxCollider>();
            scoutSpawn.GetComponent<BoxCollider>().isTrigger = true;

            bool heightCorrect = false;

            while (!heightCorrect)
            {
                scoutStar.transform.position = scoutStar.transform.position + new Vector3(0, 0.1f, 0);
                RaycastHit hit;
                if (Physics.Raycast(scoutStar.transform.position, scoutSpawn.transform.position - scoutStar.transform.position, out hit))
                {
                    // Check if the ray hits the target object
                    if (hit.collider.gameObject == scoutSpawn)
                    {
                        heightCorrect = true;
                    }
                }
            }
            scoutStar.transform.position = scoutStar.transform.position + new Vector3(0, 3f, 0);
        }

        public void RunOpeningScene()
        {
            character.GetComponent<StarterAssetsInputs>().cursorLocked = true;
            if (skipOpeningScene)
            {
                PlayerSetup();
            }
            else
            {
                openingSceneUi.SetActive(true);
                StartCoroutine(OpeningSceneCoroutine());
            }
        }

        private IEnumerator OpeningSceneCoroutine()
        {
            camRoot.transform.position = star.transform.position + new Vector3(0, 50, -30); //Start
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -20), 4).setEaseInOutSine(); //Move down
            yield return wait4p5Seconds;
            LeanTween.move(camRoot, star.transform.position + new Vector3(0, 0, -4.2f), 3.5f).setEaseInOutSine(); //Star zoom
            yield return wait4Seconds;
            openingSceneUi.GetComponent<OpeningSceneUi>().starText.runAnimation();
            yield return wait0p4Seconds;
            star.starSpin();
            yield return wait3p2Seconds;
            LeanTween.move(camRoot, scoutStar.transform.position + new Vector3(0, 0, -30f), 3f).setEaseInOutSine(); //Behind scout
            LeanTween.rotate(camRoot, new Vector3(20, 0, 0), 2f).setEaseInOutSine(); //Behind scout
            yield return wait3p2Seconds;

            float timeToBack = levelController.chunks.Count * 1.4f; // in first and last second do ease
            LeanTween.move(camRoot, scoutSpawn.transform.position + new Vector3(0, 0, -10f), timeToBack).setEaseInOutQuad(); //Behind scout
            yield return new WaitForSecondsRealtime(timeToBack);
            LeanTween.move(camRoot, character.transform.position + new Vector3(0, 1.375f, -10f), 3).setEaseInOutSine(); //Behind scout
            LeanTween.rotate(camRoot, new Vector3(0, 0, 0), 2f).setEaseInOutSine(); //Behind scout
            yield return wait3p2Seconds;
            LeanTween.move(camRoot, character.transform.position + new Vector3(0, 1.375f, 0), 3).setEaseInOutSine();

            //CUSTSCENE MOVE CAM SETTINGS
            character.setCameraSettingsOverTime(savedCameraSettings, 3);
            //LeanTween.value(this.gameObject, character.getCameraSettings(), savedFov, 3).setOnUpdate((float fovValue) => { character.setCameraSettings(fovValue); });



            yield return wait2Seconds;

            if (character.GetComponentInChildren<MeshRenderer>())
            {
                Material material = character.GetComponentInChildren<MeshRenderer>().material;
                Color color = material.color;
                LeanTween.value(this.gameObject, color.a, 0, 1).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });
            }

            yield return wait1Second;

            if (character.GetComponentInChildren<MeshRenderer>())
            {
                Material material = character.GetComponentInChildren<MeshRenderer>().material;
                Color color = material.color;
                LeanTween.value(this.gameObject, color.a, 255, 0).setOnUpdate((float alphaValue) => { color.a = alphaValue; material.color = color; });
            }
            PlayerSetup();
        }

        public void GetReferenceVariables()
        {
            star = GameObject.Find("Star").GetComponent<Star>();
            playerSpawn = GameObject.Find("PlayerSpawn");
            camRoot = GameObject.Find("EtraPlayerCameraRoot");
            cursorCanvas = GameObject.Find("CursorCanvas");
            levelController = GetComponentInChildren<LevelController>();
            character = EtraCharacterMainController.Instance;
        }

        private void PlayerSetup()
        {
            EtraStandardMenusManager menusManager = FindObjectOfType<EtraStandardMenusManager>();
            menusManager.canFreeze = true;
            character.enableAllActiveAbilities();
            character.setCameraSettings(savedCameraSettings);
            LeanTween.move(camRoot, character.transform.position + new Vector3(0, 1.375f, 0), 0).setEaseInOutSine();

            cursorCanvas.SetActive(true);
            foreach (AbilityOrItemPickup a in pickups)
            {
                a.gameObject.SetActive(true);
            }
            foreach (AnimationTriggerPickup a in animPickups)
            {
                a.gameObject.SetActive(true);
            }
            openingSceneUi.SetActive(true);
            nonGamerTutorialUi.gameObject.SetActive(true);

            if (character.etraFPSUsableItemManager)
            {
                character.etraFPSUsableItemManager.instatiateItemAtStart();
            }

            AudioManager[] managers = FindObjectsOfType<AudioManager>();

            foreach (AudioManager manager in managers)
            {
                if (manager.silenceSoundsUntilTutorialBegins)
                {
                    manager.stopPlayingSounds = false;
                }

            }

        }
    }
}

 */