using Etra.StandardMenus;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class StarterAssetAudioMenuSliders : MonoBehaviour
    {
        [Header("References")]
        public Slider dialogue;

        // Start is called before the first frame update
        void OnEnable()
        {
            // Load slider values from current settings
            LoadSliderValuesFromCurrentSettings();

            // Get the current audio mixer
            AudioMixer audioMixer = EtraStandardMenuSettingsFunctions.GetCurrentAudioMixer();

            // Add listeners for each slider
            dialogue.onValueChanged.AddListener((v) =>
            {
                audioMixer.SetFloat("Dialogue", Mathf.Log10(v) * 20);
                SetAudioPlayerPrefs();
            });

        }

        public static void SetAudioPlayerPrefs()
        {
            AudioMixer audioMixer = EtraStandardMenuSettingsFunctions.GetCurrentAudioMixer();
            float dialogueVolume;

            audioMixer.GetFloat("Dialogue", out dialogueVolume);
            PlayerPrefs.SetFloat("etraDialogueSliderVolume", dialogueVolume);
        }

        void LoadSliderValuesFromCurrentSettings()
        {
            // Get the current audio mixer
            AudioMixer audioMixer = EtraStandardMenuSettingsFunctions.GetCurrentAudioMixer();

            float dialogueVolume;

            audioMixer.GetFloat("Dialogue", out dialogueVolume);
            dialogue.value = Mathf.Pow(10, dialogueVolume / 20f);

            // Update the slider text
            dialogue.GetComponent<EtraSlider>().UpdateSliderText(dialogue.value);
        }
    }

}
