using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Gameplay_AudioMenu : EtraStandardMenu
{
    [Header("References")]
    public Slider master;
    public Slider sfx;
    public Slider music;

    // Start is called before the first frame update
    void OnEnable()
    {
        LoadSliderValuesFromCurrentSettings();


        AudioMixer audioMixer = EtraStandardMenuSettingsFunctions.GetCurrentAudioMixer();
        //Make listeners for each slider
        master.onValueChanged.AddListener((v) => {
            audioMixer.SetFloat("Master", Mathf.Log10(v)*20);
            EtraStandardMenuSettingsFunctions.SetAudioPlayerPrefs();
        });

        sfx.onValueChanged.AddListener((v) => {
            audioMixer.SetFloat("SFX", Mathf.Log10(v) * 20);
            EtraStandardMenuSettingsFunctions.SetAudioPlayerPrefs();
        });

        music.onValueChanged.AddListener((v) => {
            audioMixer.SetFloat("Music", Mathf.Log10(v) * 20);
            EtraStandardMenuSettingsFunctions.SetAudioPlayerPrefs();
        });
    }

    void LoadSliderValuesFromCurrentSettings()
    {
        AudioMixer audioMixer = EtraStandardMenuSettingsFunctions.GetCurrentAudioMixer();
        //Read all the default sliders.
        float masterVolume;
        float sfxVolume;
        float musicVolume;

        audioMixer.GetFloat("Master", out masterVolume);
        master.value = Mathf.Pow(10, masterVolume / 20f);

        audioMixer.GetFloat("SFX", out sfxVolume);
        sfx.value = Mathf.Pow(10, sfxVolume / 20f);

        audioMixer.GetFloat("Music", out musicVolume);
        music.value = Mathf.Pow(10, musicVolume / 20f);

        master.GetComponent<EtraSlider>().UpdateSliderText(master.value);
        sfx.GetComponent<EtraSlider>().UpdateSliderText(sfx.value);
        music.GetComponent<EtraSlider>().UpdateSliderText(music.value);

    }

}
