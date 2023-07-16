using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Etra.StandardMenus
{
    public class EtraAudioMenu : EtraStandardMenu
    {
        [Header("References")]
        public Slider master;
        public Slider sfx;
        public Slider music;

        // Start is called before the first frame update
        void OnEnable()
        {
            // Load slider values from current settings
            LoadSliderValuesFromCurrentSettings();

            // Get the current audio mixer
            AudioMixer audioMixer = EtraStandardMenuSettingsFunctions.GetCurrentAudioMixer();

            // Add listeners for each slider
            master.onValueChanged.AddListener((v) =>
            {
                audioMixer.SetFloat("Master", Mathf.Log10(v) * 20);
                EtraStandardMenuSettingsFunctions.SetAudioPlayerPrefs();
            });

            sfx.onValueChanged.AddListener((v) =>
            {
                audioMixer.SetFloat("SFX", Mathf.Log10(v) * 20);
                EtraStandardMenuSettingsFunctions.SetAudioPlayerPrefs();
            });

            music.onValueChanged.AddListener((v) =>
            {
                audioMixer.SetFloat("Music", Mathf.Log10(v) * 20);
                EtraStandardMenuSettingsFunctions.SetAudioPlayerPrefs();
            });
        }

        // Load slider values from current audio settings
        void LoadSliderValuesFromCurrentSettings()
        {
            // Get the current audio mixer
            AudioMixer audioMixer = EtraStandardMenuSettingsFunctions.GetCurrentAudioMixer();

            // Read the current volume levels from the audio mixer
            float masterVolume;
            float sfxVolume;
            float musicVolume;

            audioMixer.GetFloat("Master", out masterVolume);
            master.value = Mathf.Pow(10, masterVolume / 20f);

            audioMixer.GetFloat("SFX", out sfxVolume);
            sfx.value = Mathf.Pow(10, sfxVolume / 20f);

            audioMixer.GetFloat("Music", out musicVolume);
            music.value = Mathf.Pow(10, musicVolume / 20f);

            // Update the slider text
            master.GetComponent<EtraSlider>().UpdateSliderText(master.value);
            sfx.GetComponent<EtraSlider>().UpdateSliderText(sfx.value);
            music.GetComponent<EtraSlider>().UpdateSliderText(music.value);
        }
    }
}
