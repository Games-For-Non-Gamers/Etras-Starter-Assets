using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source.Camera;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class Gameplay_GameplayMenu : EtraStandardMenu
{
    public Slider mouseSensitivity;
    public Slider joystickSensitivity;
    public Toggle invertYToggle;
    public Toggle reticleToggle;
    public Toggle screenShakeToggle;


    private void OnEnable()
    {
        mouseSensitivity.interactable= false;
        joystickSensitivity.interactable = false;
        invertYToggle.interactable = false;

        if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
        {
            mouseSensitivity.interactable = true;
            joystickSensitivity.interactable = true;
            invertYToggle.interactable = true;

            mouseSensitivity.onValueChanged.AddListener((v) => {
                EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>().mouseSensitivity = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });

            joystickSensitivity.onValueChanged.AddListener((v) => {
                EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>().joystickSensitivity = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });

            invertYToggle.onValueChanged.AddListener((v) => {
                EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>().invertY = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });
        }

        reticleToggle.interactable = false;
        if (GameObject.Find("Cursor"))
        {
            reticleToggle.interactable = true;
            reticleToggle.onValueChanged.AddListener((v) => {
                GameObject.Find("Cursor").GetComponent<Image>().enabled = v;
                LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
            });
        }
        else
        {
            reticleToggle.interactable = false;
            reticleToggle.isOn = false;
        }

        screenShakeToggle.interactable = true;
        screenShakeToggle.onValueChanged.AddListener((v) => {
            CinemachineShake.Instance.shakeEnabled = v;
            LoadSavedEtraStandardGameplayMenuSettings.SetGameplayPlayerPrefs();
        });

        LoadSavedEtraStandardGameplayMenuSettings.LoadGameplayPlayerPrefs();
        LoadUiValuesFromCurrentSettings();

    }

    
    void LoadUiValuesFromCurrentSettings()
    {

        if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
        {
            ABILITY_CameraMovement camAbility = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
            mouseSensitivity.value = camAbility.mouseSensitivity;
            joystickSensitivity.value = camAbility.joystickSensitivity;
            invertYToggle.isOn = camAbility.invertY;
        }

        if (GameObject.Find("Cursor"))
        {
            if (GameObject.Find("Cursor").activeInHierarchy && GameObject.Find("Cursor").GetComponent<Image>().enabled)
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
            if (CinemachineShake.Instance.shakeEnabled)
            {
                screenShakeToggle.isOn = true;
            }
            else
            {
                screenShakeToggle.isOn = false;
            }
        }

    }
 
}
