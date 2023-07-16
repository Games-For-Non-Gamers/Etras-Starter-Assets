using Etra.StandardMenus;
using UnityEngine;
using UnityEngine.UI;
using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source.Camera;

namespace Etra.StarterAssets
{
    public class LoadSavedEtraStandardGameplayMenuSettings : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            LoadGameplayPlayerPrefs();
        }

        public static void SetGameplayPlayerPrefs()
        {
            // Set gameplay-related player preferences

            if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
            {
                // Set camera movement sensitivity and Y-axis inversion
                ABILITY_CameraMovement camAbility = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
                PlayerPrefs.SetFloat("etraMouseSensitivity", camAbility.mouseSensitivity);
                PlayerPrefs.SetFloat("etraJoystickSensitivity", camAbility.joystickSensitivity);
                int invertYValue = camAbility.invertY ? 1 : 0;
                PlayerPrefs.SetInt("etraInvertYToggle", invertYValue);
            }

            if (GameObject.Find("Cursor"))
            {
                // Set reticle visibility toggle
                PlayerPrefs.SetInt("etraReticleToggle", GameObject.Find("Cursor").activeInHierarchy && GameObject.Find("Cursor").GetComponent<Image>().enabled ? 1 : 0);
            }

            if (UnityEngine.Object.FindObjectOfType<CinemachineShake>())
            {
                // Set screen shake toggle
                PlayerPrefs.SetInt("etraScreenShakeToggle", UnityEngine.Object.FindObjectOfType<CinemachineShake>().shakeEnabled ? 1 : 0);
            }
        }

        public static void LoadGameplayPlayerPrefs()
        {
            // Load gameplay-related player preferences

            if (!PlayerPrefs.HasKey("etraMouseSensitivity"))
            {
                // Set player preferences if not already set
                SetGameplayPlayerPrefs();
            }

            if (FindObjectOfType<EtraCharacterMainController>().etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
            {
                // Load camera movement sensitivity and Y-axis inversion
                ABILITY_CameraMovement camAbility = FindObjectOfType<EtraCharacterMainController>().etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
                camAbility.mouseSensitivity = PlayerPrefs.GetFloat("etraMouseSensitivity");
                camAbility.joystickSensitivity = PlayerPrefs.GetFloat("etraJoystickSensitivity");
                camAbility.invertY = PlayerPrefs.GetInt("etraInvertYToggle") == 1;
            }

            if (GameObject.Find("Cursor"))
            {
                // Load reticle visibility toggle
                GameObject.Find("Cursor").GetComponent<Image>().enabled = PlayerPrefs.GetInt("etraReticleToggle") == 1;
            }

            CinemachineShake[] shakeScripts = UnityEngine.Object.FindObjectsOfType<CinemachineShake>();

            foreach (CinemachineShake shake in shakeScripts)
            {
                // Load screen shake toggle
                shake.shakeEnabled = PlayerPrefs.GetInt("etraScreenShakeToggle") == 1;
            }
        }
    }
}