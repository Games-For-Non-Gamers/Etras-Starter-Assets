using Etra.StarterAssets.Source;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay_GraphicsMenu : EtraStandardMenu
{
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
        bool isWindowed = Screen.fullScreenMode == FullScreenMode.Windowed;
        bool isFullScreenWindow = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        int systemWidth = Display.main.systemWidth;
        int systemHeight = Display.main.systemHeight;

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