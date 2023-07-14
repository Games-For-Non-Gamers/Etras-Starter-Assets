using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Gameplay_AudioMenu : EtraStandardMenu
{
    [Header("Basic")]
    public AudioMixer audioMixer;

    [Header("References")]
    public Slider master;
    public Slider sfx;
    public Slider music;

    // Start is called before the first frame update
    void Start()
    {
        if (audioMixer == null)
        {
            Debug.LogError("You must link an audio mixer to the Gameplay_AudioMenu script to use it.");
            this.enabled= false;
        }


        if (!PlayerPrefs.HasKey("etraMasterSliderVolume"))
        {
            CreateAudioPrefs();
        }

        LoadSliderValuesFromPrefs();

        //Make listeners for each slider
        master.onValueChanged.AddListener((v) => {
            audioMixer.SetFloat("Master", Mathf.Log10(v)*20);
            PlayerPrefs.SetFloat("etraMasterSliderVolume", v);
        });

        sfx.onValueChanged.AddListener((v) => {
            audioMixer.SetFloat("SFX", Mathf.Log10(v) * 20);
            PlayerPrefs.SetFloat("etraSFXSliderVolume", v);
        });

        music.onValueChanged.AddListener((v) => {
            audioMixer.SetFloat("Music", Mathf.Log10(v) * 20);
            PlayerPrefs.SetFloat("etraMusicSliderVolume", v);
        });

    }

    void CreateAudioPrefs()
    {
        //Read all the default sliders.
        float masterVolume;
        float sfxVolume;
        float musicVolume;

        audioMixer.GetFloat("Master", out masterVolume);
        PlayerPrefs.SetFloat("etraMasterSliderVolume", Mathf.Pow(10, masterVolume / 20));

        audioMixer.GetFloat("SFX", out sfxVolume);
        PlayerPrefs.SetFloat("etraSFXSliderVolume", Mathf.Pow(10, sfxVolume / 20));

        audioMixer.GetFloat("Music", out musicVolume);
        PlayerPrefs.SetFloat("etraMusicSliderVolume", Mathf.Pow(10, musicVolume / 20));
    }

    void LoadSliderValuesFromPrefs()
    {
        master.value = PlayerPrefs.GetFloat("etraMasterSliderVolume");
        sfx.value = PlayerPrefs.GetFloat("etraSFXSliderVolume");
        music.value = PlayerPrefs.GetFloat("etraMusicSliderVolume");

        audioMixer.SetFloat("Master", Mathf.Log10(master.value) * 20);
        PlayerPrefs.SetFloat("etraMasterSliderVolume", master.value);
        audioMixer.SetFloat("SFX", Mathf.Log10(sfx.value) * 20);
        PlayerPrefs.SetFloat("etraSFXSliderVolume", sfx.value);
        audioMixer.SetFloat("Music", Mathf.Log10(music.value) * 20);
        PlayerPrefs.SetFloat("etraMusicSliderVolume", music.value);
    }

}
