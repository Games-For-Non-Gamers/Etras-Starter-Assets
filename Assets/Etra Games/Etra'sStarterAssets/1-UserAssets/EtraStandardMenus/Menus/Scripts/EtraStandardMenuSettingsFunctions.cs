using UnityEngine;
using UnityEngine.Audio;

namespace Etra.StandardMenus
{
    public static class EtraStandardMenuSettingsFunctions
    {
        // This class manages functions for the graphics and audio settings set in the EtraStandardMenus.
        // For Gameplay settings in Etra's Starter Assets, go to the LoadSavedEtraStandardGameplayMenuSettings script.

        #region Graphics Functions
        // Automatically selects the quality settings based on system specifications
        public static void AutomaticallySelectQuality()
        {
            int totalSystemMemoryMB = SystemInfo.systemMemorySize;
            string graphicsDeviceName = SystemInfo.graphicsDeviceName;

            // Add your own benchmark conditions
            if (totalSystemMemoryMB < 2048)
            {
                QualitySettings.SetQualityLevel(0);
                QualitySettings.vSyncCount = 0;
            }
            else if (totalSystemMemoryMB >= 2048 && totalSystemMemoryMB < 8192)
            {
                // Medium-end PC
                QualitySettings.SetQualityLevel(3);
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                // High-end PC
                QualitySettings.SetQualityLevel(5);
                QualitySettings.vSyncCount = 1;
            }

            if (graphicsDeviceName.Contains("NVIDIA") || graphicsDeviceName.Contains("AMD"))
            {
                QualitySettings.SetQualityLevel(5);
                QualitySettings.vSyncCount = 1;
            }

            SetGraphicsPlayerPrefs();
        }

        // Sets the graphics player prefs based on current settings
        public static void SetGraphicsPlayerPrefs()
        {
            PlayerPrefs.SetInt("UnityGraphicsQuality", QualitySettings.GetQualityLevel());
            PlayerPrefs.SetInt("etraScreenWidth", Screen.width);
            PlayerPrefs.SetInt("etraScreenHeight", Screen.height);
            PlayerPrefs.SetInt("etraScreenMode", (int)Screen.fullScreenMode);
            PlayerPrefs.SetInt("etraVSyncCount", QualitySettings.vSyncCount);
            PlayerPrefs.Save();
        }

        // Loads the graphics player prefs and applies the settings
        public static void LoadGraphicsPlayerPrefs()
        {
            if (!PlayerPrefs.HasKey("etraScreenWidth"))
            {
                SetGraphicsPlayerPrefs();
            }

            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("UnityGraphicsQuality"));
            Screen.SetResolution(PlayerPrefs.GetInt("etraScreenWidth"), PlayerPrefs.GetInt("etraScreenHeight"), (FullScreenMode)PlayerPrefs.GetInt("etraScreenMode"));
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("etraVSyncCount");

            if (UnityEngine.Object.FindObjectOfType<EtraStandardMenusManager>())
            {
                if (UnityEngine.Object.FindObjectOfType<EtraStandardMenusManager>().graphicsMenu != null)
                {
                    bool rememberToInactive = false;
                    if (!UnityEngine.Object.FindObjectOfType<EtraStandardMenusManager>().graphicsMenu.activeInHierarchy)
                    {
                        UnityEngine.Object.FindObjectOfType<EtraStandardMenusManager>().graphicsMenu.SetActive(true);
                        rememberToInactive = true;
                    }

                    if (PlayerPrefs.GetInt("etraShowFps") == 1)
                    {
                        UnityEngine.Object.FindObjectOfType<EtraGraphicsMenu>().ShowInGameFps();
                    }
                    else
                    {
                        UnityEngine.Object.FindObjectOfType<EtraGraphicsMenu>().HideInGameFps();
                    }

                    if (rememberToInactive)
                    {
                        UnityEngine.Object.FindObjectOfType<EtraStandardMenusManager>().graphicsMenu.SetActive(false);
                    }
                }
            }
        }

        #endregion

        #region Audio Functions

        // Gets the current audio mixer in the scene
        public static AudioMixer GetCurrentAudioMixer()
        {
            AudioMixer audioMixer;
            AudioSource audioSource = UnityEngine.Object.FindObjectOfType<AudioSource>();

            if (audioSource != null)
            {
                audioMixer = audioSource.outputAudioMixerGroup.audioMixer;

                if (audioMixer != null)
                {
                    return audioMixer;
                }

                Debug.LogError("No Audio Sources found in the scene. Volume settings cannot be automatically set.");
                return null;
            }
            else
            {
                Debug.LogError("No Audio Sources found in the scene. Volume settings cannot be automatically set.");
                return null;
            }
        }

        // Sets the audio player prefs based on current settings
        public static void SetAudioPlayerPrefs()
        {
            AudioMixer audioMixer = GetCurrentAudioMixer();

            // Read all the default sliders
            float masterVolume;
            float sfxVolume;
            float musicVolume;

            audioMixer.GetFloat("Master", out masterVolume);
            PlayerPrefs.SetFloat("etraMasterSliderVolume", masterVolume);

            audioMixer.GetFloat("SFX", out sfxVolume);
            PlayerPrefs.SetFloat("etraSFXSliderVolume", sfxVolume);

            audioMixer.GetFloat("Music", out musicVolume);
            PlayerPrefs.SetFloat("etraMusicSliderVolume", musicVolume);
        }

        // Loads the audio player prefs and applies the settings
        public static void LoadAudioPlayerPrefs()
        {
            if (!PlayerPrefs.HasKey("etraMasterSliderVolume"))
            {
                SetAudioPlayerPrefs();
            }

            AudioMixer audioMixer = GetCurrentAudioMixer();

            audioMixer.SetFloat("Master", PlayerPrefs.GetFloat("etraMasterSliderVolume"));
            audioMixer.SetFloat("SFX", PlayerPrefs.GetFloat("etraSFXSliderVolume"));
            audioMixer.SetFloat("Music", PlayerPrefs.GetFloat("etraMusicSliderVolume"));
        }

        #endregion
    }
}
