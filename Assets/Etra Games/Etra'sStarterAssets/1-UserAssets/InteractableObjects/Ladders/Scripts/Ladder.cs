using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source.Interactions;
using EtrasStarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Etra.StarterAssets.Interactables
{
    public class Ladder : MonoBehaviour
    {

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
        ABILITY_CameraMovement cameraAbility;
        ABILITY_Jump jumpAbility;
        LadderTopTrigger ladderTopTrigger;
        // Start is called before the first frame update



        void Start()
        {
            transform.forward = transform.rotation * Vector3.forward;
            ladderBodyInteraction.isInteractable = false;
            ladderTopInteraction.isInteractable = false;
            if (LadderMovement.AnimateAndLock == ladderMovement)
            {
                ladderTopInteraction.isInteractable = true;
                ladderBodyInteraction.isInteractable = true;
            }

            mainController = FindObjectOfType<EtraCharacterMainController>();
            movementAbility = mainController.etraAbilityManager.GetComponent<ABILITY_CharacterMovement>(); //Add checks/warnings for the abilities
            jumpAbility = mainController.etraAbilityManager.GetComponent<ABILITY_Jump>();
            cameraAbility = mainController.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
            ladderTopTrigger =GetComponentInChildren<LadderTopTrigger>();
            camRoot = GameObject.Find("EtraPlayerCameraRoot");
        }



        LayerMask allLayersMask = ~0;
        // Update is called once per frame
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


        GameObject camRoot;

        public void attachToLadder()
        {
            attachToLadder(false);
        }

        public void attachToLadder(bool fromAbove)
        {
            ladderBodyInteraction.isInteractable = false;
            switch (ladderMovement)
            {
                case LadderMovement.FreeMove:
                    //Didn't really detail this patch much so it might be buggy, feel free to though yourself!
                    mainController.gravityActive = false;
                    jumpAbility.lockJump = false;
                    ladderFinalAttach();

                    break;
                case LadderMovement.AnimateAndLock:
                    mainController.disableAllActiveAbilities();
                    mainController.etraAbilityManager.GetComponent<ABILITY_Sprint>().abilityEnabled = true;
                    mainController.gravityActive = false;
                    jumpAbility.lockJump = false;
                    StartCoroutine(attachToLadderAnimation(fromAbove));
                    break;
            }
        }

        IEnumerator attachToLadderAnimation(bool fromAbove)
        {
            ladderTopInteraction.isInteractable = false;

            Vector3 ladderForward = -transform.forward; // Get the ladder's forward vector and reverse it

            Quaternion targetRotation = Quaternion.LookRotation(ladderForward, Vector3.up);
            if (!fromAbove)
            {
                Vector3 targetPosition = transform.position - ladderForward * 0.4f;

                LeanTween.move(mainController.gameObject, new Vector3(targetPosition.x, mainController.transform.position.y, targetPosition.z), 2f).setEaseInOutSine();



                LeanTween.rotate(camRoot, targetRotation.eulerAngles, 2f).setEaseInOutSine();
                yield return new WaitForSeconds(2);
            }
            else
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

                //If moving at ladder
                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0,0.2f,0), movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) 
                    && hit.collider.gameObject == this.gameObject 
                    && movementAbility.passedMovementInput != Vector2.zero)
                {
                    
                    mainController.addConstantForceToEtraCharacter(Vector3.up * ladderClimbSpeed);
                    isClimbingLadder = true;
                }

                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0, 0.2f, 0), -movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) 
                    && hit.collider.gameObject == this.gameObject
                    && movementAbility.passedMovementInput != Vector2.zero)
                {

                    if (!mainController.Grounded)
                    {
                        movementAbility.preventMovement = true;
                        mainController.addConstantForceToEtraCharacter(Vector3.down * ladderClimbSpeed);
                        isClimbingLadder = true;
                    }
                    else //grounded is true
                    {
                        movementAbility.preventMovement = false;
                        if (ladderMovement == LadderMovement.AnimateAndLock && attatchedToLadder)
                        {
                            StartCoroutine(detachFromLadder());
                        }
                    }

                }

                if (isClimbingLadder)
                {
                    float nextStepThreshold;
                    if (movementAbility.isSprinting)
                    {
                        nextStepThreshold = 1/ ladderSprintClimbSpeed;
                    }
                    else
                    {
                        nextStepThreshold = 1/ ladderWalkClimbSpeed;
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
                StartCoroutine(detachFromLadder());
            }
        }
        IEnumerator detachFromLadder()
        {
            if (ladderMovement == LadderMovement.AnimateAndLock && attatchedToLadder)
            {
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


            ladderDetatch.Invoke();

            if (ladderTopTrigger.isOverlappingPlayer && jumpAtEnd == JumpAtEnd.JumpAtEnd)
            {
                jumpAbility.jump();
            }
        }

    }

}
