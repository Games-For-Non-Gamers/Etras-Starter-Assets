using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source.Camera;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Gameplay_GameplayMenu : EtraStandardMenu
{
    public Slider mouseSensitivity;
    public Slider joystickSensitivity;
    public Toggle reticleToggle;
    public Toggle screenShakeToggle;
    public Toggle invertYToggle;

    private void Start()
    {
        mouseSensitivity.interactable= false;
        joystickSensitivity.interactable = false;
        reticleToggle.interactable = false;
        screenShakeToggle.interactable = false;
        invertYToggle.interactable = false;

        if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
        {
            mouseSensitivity.interactable = true;
            joystickSensitivity.interactable = true;
            invertYToggle.interactable = true;

            mouseSensitivity.onValueChanged.AddListener((v) => {
                EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>().mouseSensitivity = v;
            });

            joystickSensitivity.onValueChanged.AddListener((v) => {
                EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>().joystickSensitivity = v;
            });

            invertYToggle.onValueChanged.AddListener((v) => {
                EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>().invertY = v;
            });
        }

        if (GameObject.Find("Cursor"))
        {
            reticleToggle.interactable = true;
            reticleToggle.onValueChanged.AddListener((v) => {
                GameObject.Find("Cursor").GetComponent<Image>().enabled = v;
            });
        }

        screenShakeToggle.onValueChanged.AddListener((v) => {
            screenShakeToggle.interactable = true;
            CinemachineShake.Instance.shakeEnabled = v;
        });


        if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
        {
            ABILITY_CameraMovement camAbility = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
            mouseSensitivity.value = camAbility.mouseSensitivity;
            joystickSensitivity.value = camAbility.joystickSensitivity;
            invertYToggle.isOn = false;
        }
        else
        {
            mouseSensitivity.interactable = false;
            joystickSensitivity.interactable = false;
        }


        if (GameObject.Find("Cursor"))
        {
            reticleToggle.interactable = true;
            if (GameObject.Find("Cursor").activeInHierarchy && GameObject.Find("Cursor").GetComponent<Image>().enabled)
            {
                reticleToggle.isOn = true;
            }
            else
            {
                reticleToggle.isOn = false;
            }
           
        }
        else
        {
            reticleToggle.interactable = false;
            reticleToggle.isOn = false;
        }

        screenShakeToggle.interactable = true;
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
