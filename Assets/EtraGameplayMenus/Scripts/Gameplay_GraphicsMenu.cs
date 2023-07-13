
using Etra.StarterAssets.Source;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay_GraphicsMenu : EtraGameplayMenu
{

        public TMP_Dropdown quality;
        public TMP_Dropdown resolution;
        public TMP_Dropdown fullScreenMode;
        public TMP_Dropdown display;
        public Toggle showFps;
        public Toggle vsync;
        public TextMeshProUGUI fpsCounter;


    /*
    MIT License

    Copyright (c) 2020 Steffen Itterheim

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */


    #region Dialog Getters
    string GetSelectedResolution()
        {
            if (resolution.options.Count > 0)
            {
                return resolution.options[resolution.value].text;
            }
            return "0x0";
        }
        string GetResolutionString()
        {
            return GetResolutionString(Screen.width, Screen.height);
        }
        string GetResolutionString(int w, int h)
        {
            return string.Format("{0}x{1}", w, h);
        }


        FullScreenMode GetSelectedFullScreenMode()
        {
            var mode = (FullScreenMode)fullScreenMode.value;

            // In the dropdown "MaximizedWindow" (#2 in enum) was removed because it ain't working, it is now "Windowed" (#3 in enum), hence this fix
            // see: https://issuetracker.unity3d.com/issues/fullscreen-mode-maximized-window-functionality-is-broken-and-any-built-player-changes-to-non-window-mode-when-maximizing
            if (mode == FullScreenMode.MaximizedWindow)
                mode = FullScreenMode.Windowed;

            return mode;
        }
    #endregion



    #region Unity Startup

    private void Start()
    {

        
        quality.onValueChanged.AddListener((v) => {
            OnQualityLevelChanged();
        });
        
        resolution.onValueChanged.AddListener((v) => {
            OnResolutionChanged();
        });
        fullScreenMode.onValueChanged.AddListener((v) => {
            OnFullScreenModeChanged();
        });
        display.onValueChanged.AddListener((v) => {
            OnMonitorChanged();
        });
        vsync.onValueChanged.AddListener((v) => {
            if (v)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }
        });
        showFps.onValueChanged.AddListener((v) => {
            if (v)
            {
                FindObjectOfType<StarterAssetsCanvas>().enableFpsCounter();
            }
            else
            {
                FindObjectOfType<StarterAssetsCanvas>().disableFpsCounter();
            }
        });
        
    }

    //Fps counter
    private float deltaTime;
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        int fps = Mathf.RoundToInt(1.0f / deltaTime);
        fpsCounter.text = "FPS: " + fps;
    }

    void OnEnable()
        {
            PopulateDropdowns();
            ApplyCurrentSettingsToDialog();
            UpdateDialogInteractability();

        if (GameObject.FindObjectOfType<StarterAssetsCanvas>())
        {
            showFps.interactable = true;
            showFps.isOn = FindObjectOfType<StarterAssetsCanvas>().fpsCounterOn;
        }
        else
        {
            showFps.interactable = false;
            showFps.isOn = false; 
        }

        if (QualitySettings.vSyncCount > 0)
        {
            vsync.isOn = true;
        }
        else
        {
            vsync.isOn = false;
        }

        }
        #endregion

        #region PopulateDropdowns
        void PopulateDropdowns()
        {

            PopulateResolutionsDropdown();
            PopulateQualityDropdown();
            PopulateMonitorDropdown();
        }

        void PopulateResolutionsDropdown()
        {
            resolution.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
            var resolutions = Screen.resolutions;
            var isWindowed = Screen.fullScreenMode == FullScreenMode.Windowed;
            var isFullScreenWindow = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
            var systemWidth = Display.main.systemWidth;
            var systemHeight = Display.main.systemHeight;

            foreach (var res in resolutions)
            {
                var resParts = res.ToString().Split(new char[] { 'x', '@', 'H' });
                var width = int.Parse(resParts[0].Trim());
                var height = int.Parse(resParts[1].Trim());

                // skip resolutions that won't fit in windowed modes
                if (isWindowed && (width >= systemWidth || height >= systemHeight))
                    continue;
                if (isFullScreenWindow && (width > systemWidth || height > systemHeight))
                    continue;

                // resolution
                var resString = GetResolutionString(width, height);

            }

            resolution.AddOptions(options);
        }
    
        void PopulateQualityDropdown()
        {
            var options = new List<TMP_Dropdown.OptionData>();

             TMP_Dropdown.OptionData currentOption = null;
            var currentLevel = QualitySettings.GetQualityLevel();
            var qualityLevels = QualitySettings.names;
            foreach (var quality in qualityLevels)
            {
                var option = new TMP_Dropdown.OptionData(quality);
                options.Add(option);

                // remember initial quality level
                if (quality == qualityLevels[currentLevel])
                    currentOption = option;
            }

            quality.ClearOptions();
            quality.AddOptions(options);
        }

        void PopulateMonitorDropdown()
        {
            display.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();

            TMP_Dropdown.OptionData currentOption = null;
            for (int i = 0; i < Display.displays.Length; i++)
            {
                var display = Display.displays[i];

                var displayString = "Diplay " + (i + 1) + " (" + GetResolutionString(display.systemWidth, display.systemHeight) + ")";
                var option = new TMP_Dropdown.OptionData(displayString);
                options.Add(option);

                // select active display
                if (display.active)
                    currentOption = option;
            }

            display.AddOptions(options);
        }
        #endregion

        #region UpdateDialogWithCurrentSettings
        public void ApplyCurrentSettingsToDialog()
        {
            SelectCurrentResolutionDropdownItem();
            SelectCurrentFullScreenModeDropdownItem();
            SelectCurrentQualityLevelDropdownItem();
            SelectCurrentDisplayDropdownItem();
        }

        void SelectCurrentResolutionDropdownItem()
        {
            // select highest by default
            resolution.value = resolution.options.Count - 1;

            var res = GetResolutionString();
            for (int i = 0; i < resolution.options.Count; i++)
            {
                if (resolution.options[i].text == res)
                {
                    resolution.value = i;
                    break;
                }
            }
        }

        void SelectCurrentFullScreenModeDropdownItem()
        {
            var mode = Screen.fullScreenMode;
            if (Screen.fullScreenMode == FullScreenMode.MaximizedWindow)
                mode = FullScreenMode.FullScreenWindow;

            fullScreenMode.value = (int)mode;
        }

        void SelectCurrentQualityLevelDropdownItem()
        {
            quality.value = QualitySettings.GetQualityLevel();
        }
        void SelectCurrentDisplayDropdownItem()
        {
            // take the first active display
            for (int i = 0; i < Display.displays.Length; i++)
            {
                if (Display.displays[i].active)
                {
                    display.value = i;
                    break;
                }
            }
        }
        #endregion

        #region UpdateInteractability
        void UpdateDialogInteractability()
        {
            if (Application.isEditor)
            {
                // in editor mode these settings are not applicable, some can be changed through the game view's settings (ie resolution)
                resolution.interactable = false; // change this through game view
                fullScreenMode.interactable = false; // not applicable, always "windowed"
                quality.interactable = quality.options.Count > 1; // interactable if there is more than one quality level to select from
                display.interactable = false; // not applicable, same display as editor runs on unless game view is detached
            }
            else
            {
                resolution.interactable = true; // always interactable
                fullScreenMode.interactable = true; // always interactable
                quality.interactable = quality.options.Count > 1; // interactable if there is more than one quality level to select from
                display.interactable = display.options.Count > 1; // only interactable if there are multiple displays
            }
        }

        #endregion

        #region Event Handlers
        public void OnResolutionChanged()
        {
            ApplySelectedResolution();
        }


        public void OnFullScreenModeChanged()
        {
            var wasWindowed = Screen.fullScreenMode == FullScreenMode.Windowed;

            var mode = GetSelectedFullScreenMode();
            Screen.fullScreenMode = mode;

            if (mode == FullScreenMode.Windowed)
            {
                var selectedRes = GetSelectedResolution();
                var resolution = selectedRes.Split(new char[] { 'x' });
                var width = int.Parse(resolution[0]);
                var height = int.Parse(resolution[1]);
                var screenWidth = Display.main.systemWidth;
                var screenHeight = Display.main.systemHeight;
                if (width >= screenWidth || height >= screenHeight)
                {
                    var closestWidth = screenWidth;
                    var closestHeight = screenHeight;
                    foreach (var res in Screen.resolutions)
                    {
                        if (res.width < screenWidth && res.height < screenHeight)
                        {
                            closestWidth = res.width;
                            closestHeight = res.height;
                        }
                    }

                    // set to resolution closest to desktop, just one below desktop res
                    SetResolution(closestWidth, closestHeight, mode);
                }
                else
                {
                    ApplySelectedResolution();
                }
            }
            else
            {
                ApplySelectedResolution();
            }

        }

        public void OnQualityLevelChanged()
        {
            var selectedText = quality.options[quality.value].text;
            QualitySettings.SetQualityLevel(new List<string>(QualitySettings.names).IndexOf(selectedText), true);
        }

        public void OnMonitorChanged()
        {
        }
        #endregion

        #region Apply Changes
        void ApplySelectedResolution()
        {
            // in case resolution changed, we need to check whether the Hz selection still applies for the new resolution
            // if not we opt to go with the default '0' Hz
            var selectedRes = GetSelectedResolution();
            var resolution = selectedRes.Split(new char[] { 'x' });
            var width = int.Parse(resolution[0]);
            var height = int.Parse(resolution[1]);
            SetResolution(width, height, GetSelectedFullScreenMode());
        }

        void SetResolution(int width, int height, FullScreenMode mode) 
        { 
            Screen.SetResolution(width, height, mode);
        }
        #endregion

}
