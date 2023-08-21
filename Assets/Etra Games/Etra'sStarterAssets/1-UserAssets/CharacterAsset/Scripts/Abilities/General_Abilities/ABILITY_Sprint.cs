using Cinemachine;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source.Camera;
using Etra.StarterAssets.Source;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_Sprint : EtraAbilityBaseClass
    {
        // Enums for sprint type and stamina UI type
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
        public float secondsToSprint = 7f;
        public float recoveryDelay = 1f;
        public float secondsToRecover = 6f;
        bool isRecovering;
        bool completelyOutOfStamina = false;
        float currentStamina;

        [Header("Stamina Ui")]
        public StaminaUiType staminaUiType = StaminaUiType.None;
        public bool hideUiInEditor = false;

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

            if (hideUiInEditor)
            {
                switch (staminaUiType)
                {
                    case StaminaUiType.ToCenterSlider:
                        sprintStaminaSliderUi.showUi();
                        break;
                }
            }
        }

        private Coroutine staminaRecoveryCoroutine;

        public override void abilityUpdate()
        {
#if UNITY_EDITOR
            // For dynamically adjusting sprint speed in the editor
            if (abilityEnabled)
            {
                movementAbility.sprintSpeed = sprintSpeed;
            }
#endif

            if (usingStamina)
            {
                // Handle stamina UI based on the selected UI type
                switch (staminaUiType)
                {
                    case StaminaUiType.ToCenterSlider:
                        sprintStaminaSliderUi.sliderValue = currentStamina / secondsToSprint;
                        if (movementAbility.isSprinting)
                        {
                            sprintStaminaSliderUi.SetGuyIconToSprint(true);
                        }
                        else
                        {
                            sprintStaminaSliderUi.SetGuyIconToSprint(false);
                        }
                        break;
                }

                switch (sprintType)
                {
                    case SprintType.Hold:
                        if (_input.sprint && currentStamina > 0 && !completelyOutOfStamina && movementAbility.passedMovementInput != Vector2.zero)
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
                                if (staminaRecoveryCoroutine != null)
                                {
                                    StopCoroutine(staminaRecoveryCoroutine);
                                }
                                staminaRecoveryCoroutine = StartCoroutine(StartStaminaRecovery());
                            }
                        }
                        break;

                    case SprintType.Toggle:
                        if (_input.sprint && !sprintInputHeld && currentStamina > 0 && !completelyOutOfStamina)
                        {
                            sprintInputHeld = true;
                            movementAbility.isSprinting = !movementAbility.isSprinting;
                        }
                        else if (!_input.sprint)
                        {
                            sprintInputHeld = false;
                        }

                        if (movementAbility.isSprinting && movementAbility.passedMovementInput != Vector2.zero)
                        {
                            currentStamina -= Time.deltaTime;
                            if (currentStamina <= 0)
                            {
                                movementAbility.isSprinting = false;
                                completelyOutOfStamina = true;
                            }
                        }
                        else
                        {
                            if (currentStamina < secondsToSprint && !isRecovering)
                            {
                                if (staminaRecoveryCoroutine != null)
                                {
                                    StopCoroutine(staminaRecoveryCoroutine);
                                }
                                staminaRecoveryCoroutine = StartCoroutine(StartStaminaRecovery());
                            }
                        }
                        break;
                }

                if (movementAbility.isSprinting)
                {
                    if (staminaRecoveryCoroutine != null)
                    {
                        StopCoroutine(staminaRecoveryCoroutine);
                        isRecovering = false;
                    }
                }
                else
                {
                    if (staminaRecoveryCoroutine == null)
                    {
                        staminaRecoveryCoroutine = StartCoroutine(StartStaminaRecovery());
                    }
                }

                if (movementAbility.passedMovementInput == Vector2.zero)
                {
                    movementAbility.isSprinting = false;
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
            if (completelyOutOfStamina)
            {
                StartCoroutine(OutOfStaminaAnimation());
            }

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

        IEnumerator OutOfStaminaAnimation()
        {
            switch (staminaUiType)
            {
                case StaminaUiType.ToCenterSlider:
                    sprintStaminaSliderUi.SetOutOfStaminaColors();
                    while (completelyOutOfStamina)
                    {
                        sprintStaminaSliderUi.sliderBar.SetActive(true);
                        yield return new WaitForSeconds(0.15f);
                        sprintStaminaSliderUi.sliderBar.SetActive(false);
                        yield return new WaitForSeconds(0.15f);
                    }
                    sprintStaminaSliderUi.sliderBar.SetActive(true);
                    sprintStaminaSliderUi.SetAsDefaultColors();
                    break;
            }
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

            if (hideUiInEditor)
            {
                switch (staminaUiType)
                {
                    case StaminaUiType.ToCenterSlider:
                        if (FindObjectOfType<SprintStaminaSliderUi>())
                        {
                            FindObjectOfType<SprintStaminaSliderUi>().hideUi();
                        }
                        break;
                }
            }
            else
            {
                switch (staminaUiType)
                {
                    case StaminaUiType.ToCenterSlider:
                        if (FindObjectOfType<SprintStaminaSliderUi>())
                        {
                            FindObjectOfType<SprintStaminaSliderUi>().showUi();
                        }
                        break;
                }
            }
        }

        public void updateUi()
        {
            // Destroy all UIs
            if (FindObjectOfType<SprintStaminaSliderUi>())
            {
                DestroyImmediate(FindObjectOfType<SprintStaminaSliderUi>().gameObject);
            }

            // Add new ones if needed
            switch (staminaUiType)
            {
                case StaminaUiType.ToCenterSlider:
                    if (this.gameObject.name == "Tempcube") { return; }
                    transform.parent.GetComponent<EtraCharacterMainController>().setChildObjects();
                    EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("SprintStaminaToCenterSliderUi", gameObject.transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.transform, false, Vector3.zero, Quaternion.identity, Vector3.one);
                    sprintStaminaSliderUi = FindObjectOfType<SprintStaminaSliderUi>();
                    break;
            }
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ABILITY_Sprint))]
    public class ABILITY_SprintEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ABILITY_Sprint script = (ABILITY_Sprint)target;

            if (GUILayout.Button("Update/Reset UI Type"))
            {
                script.updateUi();
            }
        }
    }
#endif

}
