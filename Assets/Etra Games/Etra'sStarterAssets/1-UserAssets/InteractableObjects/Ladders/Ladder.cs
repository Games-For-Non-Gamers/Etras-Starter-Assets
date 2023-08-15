using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

namespace Etra.StarterAssets.Interactables
{
    public class Ladder : MonoBehaviour
    {
        //1) Sfx<-----Got them, gotta add them in
        //2)Add these settingzzzzzzzzzzzzzzz!

        public enum LadderAttach
        {
            AutoAttach,
            InteractAttach
        }

        public enum ClimbingLadderJump
        {
            AwayFromLadder,
            NoJumpAwayFromLadder
        }

        public enum JumpAtEnd
        {
            jumpAtEnd,
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
        public bool canClimb = true;
        public bool jumpAtEnd = true;
        public float overwrittenJumpHeight = 0;

        [Header("Events")]
        public UnityEvent ladderAttach;
        public UnityEvent ladderDetatch;
        public UnityEvent ladderTopReached;

        private StarterAssetsInputs _input;
        EtraCharacterMainController mainController;
        ABILITY_CharacterMovement movementAbility;
        ABILITY_Jump jumpAbility;
        LadderTopTrigger ladderTopTrigger;
        // Start is called before the first frame update
        void Start()
        {
            mainController = FindObjectOfType<EtraCharacterMainController>();

            movementAbility = mainController.etraAbilityManager.GetComponent<ABILITY_CharacterMovement>(); //Add checks/warnings for the abilities
            _input = mainController.GetComponent<StarterAssetsInputs>();
            jumpAbility = mainController.etraAbilityManager.GetComponent<ABILITY_Jump>();
            ladderTopTrigger =GetComponentInChildren<LadderTopTrigger>();
        }

        LayerMask allLayersMask = ~0;
        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                mainController.gravityActive = false;
                jumpAbility.lockJump = false;
                ladderAttach.Invoke();
            }
        }


        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && canClimb)
            {
                mainController.gravityActive = false;

                if (jumpAbility.jumpInput)
                {
                    jumpAbility.jumpLaunch(this.transform.forward , jumpAwayFromLadderForce, false);
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

                //If moving at ladder
                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0,0.2f,0), movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) 
                    && hit.collider.gameObject == this.gameObject 
                    && movementAbility.passedMovementInput != Vector2.zero)
                {
                    mainController.addConstantForceToEtraCharacter(Vector3.up * ladderClimbSpeed * Time.deltaTime);
                }


                if (Physics.Raycast(other.gameObject.transform.position + new Vector3(0, 0.2f, 0), -movementAbility.movementDirection, out hit, 10f, allLayersMask, QueryTriggerInteraction.Ignore) 
                    && hit.collider.gameObject == this.gameObject
                    && movementAbility.passedMovementInput != Vector2.zero
                    && mainController.Grounded == false)
                {
                    movementAbility.preventMovement= true;
                    mainController.addConstantForceToEtraCharacter(Vector3.down * ladderClimbSpeed * Time.deltaTime);
                }
                else
                {
                    movementAbility.preventMovement = false;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                mainController.gravityActive = true;
                movementAbility.preventMovement = false;
                ladderDetatch.Invoke();
                if (ladderTopTrigger.isOverlappingPlayer && jumpAtEnd)
                {
                    if (overwrittenJumpHeight == 0)
                    {
                        jumpAbility.jump();
                    }
                    else
                    {
                        jumpAbility.jump(overwrittenJumpHeight);
                    }
                    
                }
            }
        }


    }
}
