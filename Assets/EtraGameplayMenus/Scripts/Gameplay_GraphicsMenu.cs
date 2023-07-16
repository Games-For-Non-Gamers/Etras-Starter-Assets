using Etra.StarterAssets.Source;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay_GraphicsMenu : EtraStandardMenu
{
    //Some of the following code is modified from Steffen Itterheim's UnityResolutionDialog
    /*
    MIT License

    Copyright(c) 2020 Steffen Itterheim

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */


    // UI elements
    public TMP_Dropdown quality;
    public TMP_Dropdown resolution;
    public TMP_Dropdown screenMode;
    public Toggle showFps;
    public Toggle vsync;
    public TextMeshProUGUI fpsCounter;
    // External Gameobject
    public GameObject inGameFpsCounter;

    private void Update()
    {
        CalculateFps();
    }

    private void OnEnable()
    {
        quality.onValueChanged.AddListener(OnQualityLevelChanged);
        resolution.onValueChanged.AddListener(OnResolutionChanged);
        screenMode.onValueChanged.AddListener(OnFullScreenModeChanged);
        vsync.onValueChanged.AddListener(OnVsyncChanged);
        showFps.onValueChanged.AddListener(OnShowFpsChanged);
        PopulateDropdowns();
        ApplyCurrentSettingsToDialog();
        UpdateDialogInteractability();
        SetVsyncToggle();
    }

    #region Dialog Getters
    private string GetSelectedResolution()
    {
        return resolution.options[resolution.value].text;
    }

    private string GetResolutionString()
    {
        return GetResolutionString(Screen.width, Screen.height);
    }

    private string GetResolutionString(int width, int height)
    {
        return string.Format("{0}x{1}", width, height);
    }

    private FullScreenMode GetSelectedFullScreenMode()
    {
        return (FullScreenMode)screenMode.value;
    }
    #endregion

    #region Populate Dropdowns
    private void PopulateDropdowns()
    {
        PopulateQualityDropdown();
        PopulateResolutionsDropdown();
        PopulateScreenModeDropdown();
    }

    private void PopulateQualityDropdown()
    {
        quality.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        int currentLevel = QualitySettings.GetQualityLevel();
        string[] qualityLevels = QualitySettings.names;

        foreach (string quality in qualityLevels)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(quality);
            options.Add(option);
        }

        quality.AddOptions(options);
        quality.value = currentLevel;
    }

    private void PopulateResolutionsDropdown()
    {
        resolution.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        Resolution[] resolutions = Screen.resolutions;

        foreach (Resolution res in resolutions)
        {
            int width = res.width;
            int height = res.height;
            string resString = GetResolutionString(width, height);
            options.Add(new TMP_Dropdown.OptionData(resString));
        }

        resolution.AddOptions(options);
    }

    void PopulateScreenModeDropdown()
    {
        screenMode.ClearOptions();
        string[] modeOptions = { "Exclusive FullScreen", "FullScreen Window", "Maximized Window", "Windowed" };

        foreach (string option in modeOptions)
        {
            screenMode.options.Add(new TMP_Dropdown.OptionData(option));
        }
    }
    #endregion

    #region Update Dialog with Current Settings
    private void ApplyCurrentSettingsToDialog()
    {
        SelectCurrentResolutionDropdownItem();
        SelectCurrentFullScreenModeDropdownItem();
    }

    private void SelectCurrentResolutionDropdownItem()
    {
        string currentRes = GetResolutionString();
        resolution.value = resolution.options.FindIndex(option => option.text == currentRes);
    }

    private void SelectCurrentFullScreenModeDropdownItem()
    {
        FullScreenMode mode = Screen.fullScreenMode;
        screenMode.value = (int)mode;
    }
    #endregion

    #region Update Interactability
    private void UpdateDialogInteractability()
    {
        bool isEditor = Application.isEditor;
        resolution.interactable = !isEditor;
        screenMode.interactable = !isEditor;
        quality.interactable = quality.options.Count > 1;
    }
    #endregion

    #region Event Handlers
    private void OnResolutionChanged(int value)
    {
        ApplySelectedResolution();
        EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
    }

    private void OnFullScreenModeChanged(int value)
    {
        ApplySelectedResolution();
        EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
    }

    private void OnQualityLevelChanged(int value)
    {
        QualitySettings.SetQualityLevel(value, true);
        EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
    }

    private void OnVsyncChanged(bool value)
    {
        if (value)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
    }

    private void OnShowFpsChanged(bool value)
    {
        if (value)
        {
            ShowInGameFps();
        }
        else
        {
            HideInGameFps();
        }
    }
    #endregion

    #region Apply Changes
    private void ApplySelectedResolution()
    {
        string selectedRes = GetSelectedResolution();
        string[] resParts = selectedRes.Split('x');
        int width = int.Parse(resParts[0].Trim());
        int height = int.Parse(resParts[1].Trim());
        FullScreenMode mode = GetSelectedFullScreenMode();
        Screen.SetResolution(width, height, mode);
    }
    #endregion

    #region FPS Counter
    float deltaTime;
    private void CalculateFps()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        int fps = Mathf.RoundToInt(1.0f / deltaTime);
        fpsCounter.text = "FPS: " + fps;
    }

    public void ShowInGameFps()
    {
        showFps.isOn = true;
        inGameFpsCounter.SetActive(true);
        PlayerPrefs.SetInt("etraShowFps", 1);
    }

    public void HideInGameFps()
    {
        showFps.isOn = false;
        inGameFpsCounter.SetActive(false);
        PlayerPrefs.SetInt("etraShowFps", 0);
    }
    #endregion

    #region Helper Functions
    

    private void SetVsyncToggle()
    {
        vsync.isOn = QualitySettings.vSyncCount > 0;
    }
    #endregion
}