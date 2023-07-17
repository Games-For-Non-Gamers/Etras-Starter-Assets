using Etra.StandardMenus;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class EtraGameplayMenu : EtraStandardMenu
    {
        // UI elements
        public Slider mouseSensitivity;
        public Slider joystickSensitivity;
        public Toggle invertYToggle;
        public Toggle reticleToggle;
        public Toggle screenShakeToggle;

        private void OnEnable()
        {
            // Disable interactability of UI elements by default
            mouseSensitivity.interactable = false;
            joystickSensitivity.interactable = false;
            invertYToggle.interactable = false;
            reticleToggle.interactable = false;
            screenShakeToggle.interactable = false;

            
            EtraCharacterMainController character = FindObjectOfType<EtraCharacterMainController>();
            // Enable interactability and add listeners if ABILITY_CameraMovement exists
            if (character.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
            {
                EnableCameraMovementUI();
            }

            // Enable interactability and add listeners if reticle exists
            if (GameObject.Find("Cursor"))
            {
                EnableReticleUI();
            }

            // Enable interactability and add listener for screen shake
            screenShakeToggle.interactable = true;
            screenShakeToggle.onValueChanged.AddListener((v) =>
            {
                CinemachineShake.Instance.shakeEnabled = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });

            LoadSavedEtraStandardGameplayMenuSettings.LoadGameplayPlayerPrefs();
            LoadUiValuesFromCurrentSettings();
        }

        private void EnableCameraMovementUI()
        {
            ABILITY_CameraMovement camAbility = FindObjectOfType<EtraCharacterMainController>().etraAbilityManager.GetComponent<ABILITY_CameraMovement>();

            // Enable interactability of sliders and toggle for camera movement
            mouseSensitivity.interactable = true;
            joystickSensitivity.interactable = true;
            invertYToggle.interactable = true;

            // Add listeners for mouse sensitivity, joystick sensitivity, and invert Y toggle
            mouseSensitivity.onValueChanged.AddListener((v) =>
            {
                camAbility.mouseSensitivity = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });

            joystickSensitivity.onValueChanged.AddListener((v) =>
            {
                camAbility.joystickSensitivity = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });

            invertYToggle.onValueChanged.AddListener((v) =>
            {
                camAbility.invertY = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });
        }

        private void EnableReticleUI()
        {
            // Enable interactability and add listener for reticle
            reticleToggle.interactable = true;
            reticleToggle.onValueChanged.AddListener((v) =>
            {
                GameObject.Find("Cursor").GetComponent<Image>().enabled = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });
        }

        private void LoadUiValuesFromCurrentSettings()
        {
            if (FindObjectOfType<EtraCharacterMainController>().GetComponent<ABILITY_CameraMovement>())
            {
                ABILITY_CameraMovement camAbility = FindObjectOfType<EtraCharacterMainController>().etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
                mouseSensitivity.value = camAbility.mouseSensitivity;
                joystickSensitivity.value = camAbility.joystickSensitivity;
                invertYToggle.isOn = camAbility.invertY;
            }

            if (GameObject.Find("Cursor"))
            {
                Image cursorImage = GameObject.Find("Cursor").GetComponent<Image>();

                if (cursorImage.enabled && cursorImage.gameObject.activeInHierarchy)
                {
                    reticleToggle.isOn = true;
                }
                else
                {
                    reticleToggle.isOn = false;
                }
            }

            if (CinemachineShake.Instance)
            {
                screenShakeToggle.isOn = CinemachineShake.Instance.shakeEnabled;
            }
        }
    }
}