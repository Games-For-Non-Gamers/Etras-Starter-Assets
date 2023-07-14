using UnityEngine;
using UnityEngine.Timeline;

public static class EtraStandardMenuSettingsFunctions 
{
    //This class manages functions for the graphics and audio settings set in the EtraStandardMenus.
    //For Gameplay settings in Etra's Starter Assets, go to the LoadSavedEtraStandardGameplayMenuSettings script.

    #region Graphics Functions
    public static void AutomaticallySelectQuality()
    {
        int totalSystemMemoryMB = SystemInfo.systemMemorySize;
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;

        //Add your own benchmark conditions
        if (totalSystemMemoryMB < 2048)
        {
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;
        }
        if ((totalSystemMemoryMB >= 2048 && totalSystemMemoryMB < 8192) )
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

    public static void SetGraphicsPlayerPrefs()
    {
        PlayerPrefs.SetInt("UnityGraphicsQuality", QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt("etraScreenWidth", Screen.width);
        PlayerPrefs.SetInt("etraScreenHeight", Screen.height);
        PlayerPrefs.SetInt("etraScreenMode", (int)Screen.fullScreenMode);
        PlayerPrefs.SetInt("etraVSyncCount", QualitySettings.vSyncCount);
        PlayerPrefs.Save();
    }

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
                    UnityEngine.Object.FindObjectOfType<Gameplay_GraphicsMenu>().ShowInGameFps();
                }
                else
                {
                    UnityEngine.Object.FindObjectOfType<Gameplay_GraphicsMenu>().HideInGameFps();
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


    #endregion
    //Audio


}
