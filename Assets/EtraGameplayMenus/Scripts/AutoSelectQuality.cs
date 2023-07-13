using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSelectQuality : MonoBehaviour
{
    public void automaticallySelectQuality()
    {
        int totalSystemMemoryMB = SystemInfo.systemMemorySize;
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;

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
            Debug.Log("High-end PC detected. Quality level set to Ultra.");
        }

        if (graphicsDeviceName.Contains("NVIDIA") || graphicsDeviceName.Contains("AMD"))
        {
            QualitySettings.SetQualityLevel(5);
            QualitySettings.vSyncCount = 1;
        }


        if (GetComponent<Gameplay_GraphicsMenu>())
        {
            GetComponent<Gameplay_GraphicsMenu>().ApplyCurrentSettingsToDialog();
        }
    }
}
