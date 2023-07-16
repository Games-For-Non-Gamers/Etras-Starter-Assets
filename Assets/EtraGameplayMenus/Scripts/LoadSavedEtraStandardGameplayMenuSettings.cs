using Etra.StarterAssets.Abilities;
using Etra.StarterAssets;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Etra.StarterAssets.Source.Camera;
using System.Collections;

public class LoadSavedEtraStandardGameplayMenuSettings : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
       // LoadGameplayPlayerPrefs();
    }


    public static void SetGameplayPlayerPrefs()
    {

        if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
        {
            ABILITY_CameraMovement camAbility = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
            PlayerPrefs.SetFloat("etraMouseSensitivity", camAbility.mouseSensitivity);
            PlayerPrefs.SetFloat("etraJoystickSensitivity", camAbility.joystickSensitivity);
            int invertYValue = camAbility.invertY ? 1 : 0;
            PlayerPrefs.SetInt("etraInvertYToggle", invertYValue);
        }

        if (GameObject.Find("Cursor"))
        {
            PlayerPrefs.SetInt("etraReticleToggle", GameObject.Find("Cursor").activeInHierarchy && GameObject.Find("Cursor").GetComponent<Image>().enabled ? 1 : 0);
        }

        if (UnityEngine.Object.FindObjectOfType<CinemachineShake>())
        {
            PlayerPrefs.SetInt("etraScreenShakeToggle", UnityEngine.Object.FindObjectOfType<CinemachineShake>().shakeEnabled ? 1 : 0);
        }


    }

    public static void LoadGameplayPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("etraMouseSensitivity"))
        {
            SetGameplayPlayerPrefs();
        }

        if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
        {
            ABILITY_CameraMovement camAbility = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
            camAbility.mouseSensitivity = PlayerPrefs.GetFloat("etraMouseSensitivity");
            camAbility.joystickSensitivity = PlayerPrefs.GetFloat("etraJoystickSensitivity");
            camAbility.invertY = PlayerPrefs.GetInt("etraInvertYToggle") == 1;
        }

        if (GameObject.Find("Cursor"))
        {
            GameObject.Find("Cursor").GetComponent<Image>().enabled = PlayerPrefs.GetInt("etraReticleToggle") == 1;
        }


        CinemachineShake[] shakeScripts = UnityEngine.Object.FindObjectsOfType<CinemachineShake>();

        foreach (CinemachineShake shake in shakeScripts)
        {
            shake.shakeEnabled = PlayerPrefs.GetInt("etraScreenShakeToggle") == 1;
        }
    }

}
