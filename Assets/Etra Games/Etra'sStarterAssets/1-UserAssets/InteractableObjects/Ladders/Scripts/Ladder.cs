using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source.Interactions;
using EtrasStarterAssets;

namespace Etra.StarterAssets.Interactables
{
    public class Ladder : MonoBehaviour
    {
        //Enum definitions
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
            AnimateAndLock
        }

        [Header("Basic")]
        public float ladderWalkClimbSpeed = 250;
        public float ladderSprintClimbSpeed = 500;
        public float jumpAwayFromLadderForce = 70;
        public bool canClimbLadder = true;
        public LadderAttach ladderAttachType = LadderAttach.AutoAttach;
        public JumpOffLadder jumpOffLadder = JumpOffLadder.JumpAwayFromLadder;
        public JumpAtEnd jumpAtEnd = JumpAtEnd.JumpAtEnd;
        public LadderMovement ladderMovement = LadderMovement.FreeMove;

        //private
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

        //Private references
        LayerMask allLayersMask = ~0;
        EtraCharacterMainController mainController;
        ABILITY_CharacterMovement movementAbility;
        ABILITY_CameraMovement cameraAbility;
        ABILITY_Jump jumpAbility;
        LadderTopTrigger ladderTopTrigger;
        GameObject camRoot;

        void Start()
        {
            //Set interactable and rotation for future ladder types
            transform.forward = transform.rotation * Vector3.forward;
            ladderBodyInteraction.isInteractable = false;
            ladderTopInteraction.isInteractable = false;
            if (LadderMovement.AnimateAndLock == ladderMovement)
            {
                ladderTopInteraction.isInteractable = true;
                ladderBodyInteraction.isInteractable = true;
            }

            //Set up references
            mainController = FindObjectOfType<EtraCharacterMainController>();
            movementAbility = mainController.etraAbilityManager.GetComponent<ABILITY_CharacterMovement>();
            jumpAbility = mainController.etraAbilityManager.GetComponent<ABILITY_Jump>();
            cameraAbility = mainController.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
            ladderTopTrigger = GetComponentInChildren<LadderTopTrigger>();
            camRoot = GameObject.Find("EtraPlayerCameraRoot");
        }

        //Auto attach if that is the selected option
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (ladderAttachType == LadderAttach.AutoAttach)
                {
                    attachToLadder();
                }
            }
        }

        //This is the standard veriation of the function "true" is if this is called from above for AnimateAndLock
        public void attachToLadder()
        {
            attachToLadder(false);
        }

        public void attachToLadder(bool fromAbove)
        {
            //Make the ladder body not interactable during the climb
            ladderBodyInteraction.isInteractable = false;

            switch (ladderMovement)
            {
                case LadderMovement.FreeMove:
                    //Keep movement and abilities mostly free
                    mainController.gravityActive = false;
                    jumpAbility.lockJump = false;
                    ladderFinalAttach();
                    break;
                case LadderMovement.AnimateAndLock:
                    //Enter a cutscene like mode
                    mainController.disableAllActiveAbilities();
                    mainController.etraAbilityManager.GetComponent<ABILITY_Sprint>().abilityEnabled = true;
                    mainController.gravityActive = false;
                    jumpAbility.lockJump = false;
                    StartCoroutine(attachToLadderAnimation(fromAbove));
                    break;
            }
        }

        //Animation for the attatch to ladder cutscene, rotates with ladder direction.
        IEnumerator attachToLadderAnimation(bool fromAbove)
        {
            ladderTopInteraction.isInteractable = false;

            Vector3 ladderForward = -transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(ladderForward, Vector3.up);
            if (!fromAbove) //From above animation
            {
                Vector3 targetPosition = transform.position - ladderForward * 0.4f;

                LeanTween.move(mainController.gameObject, new Vector3(targetPosition.x, mainController.transform.position.y, targetPosition.z), 2f).setEaseInOutSine();

                LeanTween.rotate(camRoot, targetRotation.eulerAngles, 2f).setEaseInOutSine();
                yield return new WaitForSeconds(2);
            }
            else //from body animation
            {
                Vector3 targetPosition = transform.position - ladderForward * 0.4f;

                LeanTween.move(mainController.gameObject, new Vector3(targetPosition.x, mainController.transform.position.y, targetPosition.z), 2f).setEaseInSine();

                LeanTween.rotate(camRoot, targetRotation.eulerAngles, 2f).setEaseInOutSine();
                yield return new WaitForSeconds(2);

                float yVal = mainController.transform.position.y - 0.5f;
                targetPosition = transform.position - ladderForward * 0.4f;

                LeanTween.move(mainController.gameObject, new Vector3(targetPosition.x, yVal, targetPosition.z), 1f).setEaseOutSine();
                yield return new WaitForSeconds(1);
            }
            cameraAbility.manualSetCharacterAndCameraRotation(targetRotation.eulerAngles);
            ladderFinalAttach();
        }

        void ladderFinalAttach()
        {
            standardAudioManager.Play("LadderAttach");
            attatchedToLadder = true;
            ladderAttach.Invoke();
        }

        //This is the location of the main climb behavior
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                switch (ladderAttachType)
                {
                    case LadderAttach.InteractAttach:
                        if (ladderMovement == LadderMovement.AnimateAndLock)
                        {
                            if (mainController.Grounded && !attatchedToLadder && mainController.gravityActive)
                            {
                                ladderBodyInteraction.isInteractable = true;
                            }
                            else
                            {
                                ladderBodyInteraction.isInteractable = false;
                            }
                        }
                        else if (!attatchedToLadder && mainController.gravityActive)
                        {
                            ladderBodyInteraction.isInteractable = true;
                        }

                        if (attatchedToLadder)
                        {
                            ladderBodyInteraction.isInteractable = false;
                        }
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


                //A rayscaset checks the direction the player in order to determine whether they should move up or down

                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0, 0.2f, 0), movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) && hit.collider.gameObject == this.gameObject && movementAbility.passedMovementInput != Vector2.zero)
                {
                    //If walking up 
                    mainController.addConstantForceToEtraCharacter(Vector3.up * ladderClimbSpeed);
                    isClimbingLadder = true;
                }

                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0, 0.2f, 0), -movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) && hit.collider.gameObject == this.gameObject && movementAbility.passedMovementInput != Vector2.zero)
                {
                    //If walking down normally
                    if (!mainController.Grounded)
                    {
                        movementAbility.preventMovement = true;
                        mainController.addConstantForceToEtraCharacter(Vector3.down * ladderClimbSpeed);
                        isClimbingLadder = true;
                    }
                    else
                    //If walking down and hitting the ground walk off the ladder
                    {
                        movementAbility.preventMovement = false;
                        if (ladderMovement == LadderMovement.AnimateAndLock && attatchedToLadder)
                        {
                            StartCoroutine(detachFromLadder());
                        }
                    }
                }

                //If climbing the ladder, play and adjust the footstep speed.
                if (isClimbingLadder)
                {
                    float nextStepThreshold;
                    if (movementAbility.isSprinting)
                    {
                        nextStepThreshold = 1 / ladderSprintClimbSpeed;
                    }
                    else
                    {
                        nextStepThreshold = 1 / ladderWalkClimbSpeed;
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

        //Regardless of type, this will trigger the detachFromLadder coroutine
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(detachFromLadder());
            }
        }

        IEnumerator detachFromLadder()
        {
            //If locked animation
            if (ladderMovement == LadderMovement.AnimateAndLock && attatchedToLadder)
            {
                //If leaving from top of ladder, do additional actions.
                if (ladderTopTrigger.isOverlappingPlayer)
                {
                    LeanTween.move(mainController.gameObject, new Vector3(this.transform.position.x, mainController.transform.position.y + 0.15f, this.transform.position.z), 0.5f).setEaseOutSine();
                    yield return new WaitForSeconds(0.5f);
                }
                mainController.enableAllActiveAbilities();
            }

            attatchedToLadder = false;
            ladderBodyInteraction.isInteractable = false;
            mainController.gravityActive = true;
            movementAbility.preventMovement = false;

            if (ladderMovement == LadderMovement.AnimateAndLock)
            {
                ladderTopInteraction.isInteractable = true;
                ladderBodyInteraction.isInteractable = true;
            }

            ladderDetatch.Invoke(); // Run the cooresponding public event/

            //Jump at end if it is true and the player is at the ladder end.
            if (ladderTopTrigger.isOverlappingPlayer && jumpAtEnd == JumpAtEnd.JumpAtEnd)
            {
                jumpAbility.jump();
            }
        }
    }
}
