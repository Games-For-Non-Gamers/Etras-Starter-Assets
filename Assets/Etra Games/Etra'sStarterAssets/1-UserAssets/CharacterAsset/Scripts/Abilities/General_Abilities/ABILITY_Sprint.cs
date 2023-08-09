using Cinemachine;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source.Camera;
using Etra.StarterAssets.Source;
using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_Sprint : EtraAbilityBaseClass
    {
        public enum SprintType
        {
            Toggle,
            Hold
        }

        public enum StaminaUiType
        {
            None,
            ToCenterSlider
        }

        [Header("Basics")]
        public SprintType sprintType = SprintType.Hold;
        public float sprintSpeed = 6;

        [Header("Stamina")]
        public bool usingStamina = false;
        public StaminaUiType staminaUiType;
        public float secondsToSprint = 7f;
        public float recoveryDelay = 1f;
        public float secondsToRecover = 6f;
        public bool isRecovering;
        public bool completelyOutOfStamina = false;
        public float currentStamina;

        StarterAssetsInputs _input;
        ABILITY_CharacterMovement movementAbility;
        SprintStaminaSliderUi sprintStaminaSliderUi;
        bool sprintInputHeld = false; // Track if sprint input is held



        public override void abilityStart()
        {
            _input = GetComponentInParent<StarterAssetsInputs>();
            movementAbility = GetComponent<ABILITY_CharacterMovement>();
            currentStamina = secondsToSprint; // Initialize stamina to maximum
            if (abilityEnabled)
            {
                movementAbility.sprintSpeed = sprintSpeed;
            }

            switch (staminaUiType)
            {
                case StaminaUiType.ToCenterSlider:
                    sprintStaminaSliderUi = FindObjectOfType<SprintStaminaSliderUi>();
                    break;
            }
        }

        private Coroutine staminaRecoveryCoroutine;

        public override void abilityUpdate()
        {
#if UNITY_EDITOR
            // For dynamically adjusting sprint speed in editor
            if (abilityEnabled)
            {
                movementAbility.sprintSpeed = sprintSpeed;
            }
#endif

            switch (staminaUiType)
            {
                case StaminaUiType.ToCenterSlider:
                    sprintStaminaSliderUi.sliderValue = currentStamina/secondsToSprint;
                    break;
            }

            if (usingStamina)
            {
                switch (sprintType)
                {
                    case SprintType.Hold:
                        if (_input.sprint && currentStamina > 0 && !completelyOutOfStamina)
                        {
                            movementAbility.isSprinting = true;
                            currentStamina -= Time.deltaTime;
                        }
                        else
                        {
                            if (currentStamina <= 0)
                            {
                                completelyOutOfStamina = true;
                            }

                            movementAbility.isSprinting = false;
                            if (currentStamina < secondsToSprint && !isRecovering)
                            {
                                if (staminaRecoveryCoroutine == null)
                                {
                                    staminaRecoveryCoroutine = StartCoroutine(StartStaminaRecovery());
                                }
                            }
                        }
                        break;

                    case SprintType.Toggle:
                        if (_input.sprint && !sprintInputHeld && currentStamina > 0 && !completelyOutOfStamina)
                        {
                            sprintInputHeld = true;
                            movementAbility.isSprinting = !movementAbility.isSprinting;
                            currentStamina -= Time.deltaTime;
                        }
                        else if (!_input.sprint)
                        {
                            if (currentStamina <= 0)
                            {
                                completelyOutOfStamina = true;
                            }
                            sprintInputHeld = false;
                        }
                        break;
                }
            }
            else
            {
                switch (sprintType)
                {
                    case SprintType.Hold:
                        if (_input.sprint)
                        {
                            movementAbility.isSprinting = true;
                        }
                        else
                        {
                            movementAbility.isSprinting = false;
                        }
                        break;

                    case SprintType.Toggle:
                        if (_input.sprint && !sprintInputHeld)
                        {
                            sprintInputHeld = true;
                            movementAbility.isSprinting = !movementAbility.isSprinting;
                        }
                        else if (!_input.sprint)
                        {
                            sprintInputHeld = false;
                        }
                        break;
                }
            }
        }

        IEnumerator StartStaminaRecovery()
        {
            isRecovering = true;
            yield return new WaitForSeconds(recoveryDelay);
            float staminaRecoveryRate = secondsToSprint / secondsToRecover;

            while (currentStamina < secondsToSprint)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                yield return null;
            }
            currentStamina = secondsToSprint;
            isRecovering = false;
            completelyOutOfStamina = false;
            staminaRecoveryCoroutine = null; // Reset the coroutine reference
        }

        private void OnValidate()
        {
            if (GetComponent<ABILITY_CharacterMovement>())
            {
                movementAbility = GetComponent<ABILITY_CharacterMovement>();
                if (abilityEnabled)
                {
                    movementAbility.sprintSpeed = sprintSpeed;
                }
            }

            switch (staminaUiType)
            {
                case StaminaUiType.ToCenterSlider:
                    if (this.gameObject.name == "Tempcube") { return; }
                    transform.parent.GetComponent<EtraCharacterMainController>().setChildObjects(); //string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot, Vector3 localScale
                    if (GameObject.Find("SprintStaminaToCenterSliderUi"))
                    {
                        DestroyImmediate(GameObject.Find("SprintStaminaToCenterSliderUi"));
                    }
                    GameObject sprintStaminaSliderUiBase = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("SprintStaminaToCenterSliderUi", gameObject.transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.transform, false, Vector3.zero, Quaternion.identity, Vector3.one);
                    sprintStaminaSliderUi = sprintStaminaSliderUiBase.GetComponent<SprintStaminaSliderUi>();

                    break;
            }

        }
    }
}
