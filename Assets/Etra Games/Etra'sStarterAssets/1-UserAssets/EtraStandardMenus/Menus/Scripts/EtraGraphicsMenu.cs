using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StandardMenus
{
    public class EtraGraphicsMenu : EtraStandardMenu
    {
        // UI elements
        public TMP_Dropdown quality;
        public TMP_Dropdown resolution;
        public TMP_Dropdown screenMode;
        public Toggle showFps;
        public Toggle vsync;
        public TextMeshProUGUI fpsCounter;
        // External GameObject
        public GameObject inGameFpsCounter;


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

        private void Update()
        {
            CalculateFps();
        }

        private void OnEnable()
        {
            // Add listeners to UI elements
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

        private void CalculateFps()
        {
            // Calculate and display FPS
            float deltaTime = Time.unscaledDeltaTime;
            float fps = 1.0f / deltaTime;
            fpsCounter.text = "FPS: " + Mathf.RoundToInt(fps);
        }

        private void OnResolutionChanged(int value)
        {
            // Apply selected resolution
            ApplySelectedResolution();
            EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
        }

        private void OnFullScreenModeChanged(int value)
        {
            // Apply selected fullscreen mode
            ApplySelectedResolution();
            EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
        }

        private void OnQualityLevelChanged(int value)
        {
            // Set quality level
            QualitySettings.SetQualityLevel(value, true);
            EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
        }

        private void OnVsyncChanged(bool value)
        {
            // Toggle v-sync
            QualitySettings.vSyncCount = value ? 1 : 0;
            EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
        }

        private void OnShowFpsChanged(bool value)
        {
            // Toggle in-game FPS display
            if (value)
                ShowInGameFps();
            else
                HideInGameFps();
        }

        private void PopulateDropdowns()
        {
            // Populate quality, resolution, and screen mode dropdowns
            PopulateQualityDropdown();
            PopulateResolutionsDropdown();
            PopulateScreenModeDropdown();
        }

        private void PopulateQualityDropdown()
        {
            // Populate quality dropdown with available options
            quality.ClearOptions();
            List<string> qualityOptions = new List<string>(QualitySettings.names);
            quality.AddOptions(qualityOptions);
            quality.value = QualitySettings.GetQualityLevel();
        }

        private void PopulateResolutionsDropdown()
        {
            // Populate resolutions dropdown with available options
            resolution.ClearOptions();
            List<string> resolutionOptions = new List<string>();
            Resolution[] resolutions = Screen.resolutions;
            foreach (Resolution res in resolutions)
                resolutionOptions.Add(res.width + "x" + res.height);
            resolution.AddOptions(resolutionOptions);
        }

        private void PopulateScreenModeDropdown()
        {
            // Populate screen mode dropdown with available options
            screenMode.ClearOptions();
            screenMode.AddOptions(new List<string> { "Exclusive FullScreen", "FullScreen Window", "Maximized Window", "Windowed" });
        }

        private void ApplyCurrentSettingsToDialog()
        {
            // Set the selected options in the dialog to match the current settings
            SelectCurrentResolutionDropdownItem();
            SelectCurrentFullScreenModeDropdownItem();
        }

        private void SelectCurrentResolutionDropdownItem()
        {
            // Select the current resolution option in the dropdown
            string currentResolution = Screen.width + "x" + Screen.height;
            resolution.value = resolution.options.FindIndex(option => option.text == currentResolution);
        }

        private void SelectCurrentFullScreenModeDropdownItem()
        {
            // Select the current fullscreen mode option in the dropdown
            screenMode.value = (int)Screen.fullScreenMode;
        }

        private void UpdateDialogInteractability()
        {
            // Update interactability of UI elements based on the platform
            bool isEditor = Application.isEditor;
            resolution.interactable = !isEditor;
            screenMode.interactable = !isEditor;
            quality.interactable = quality.options.Count > 1;
        }

        private void SetVsyncToggle()
        {
            // Set the state of the VSync toggle based on the current VSync count
            vsync.isOn = QualitySettings.vSyncCount > 0;
        }

        private void ApplySelectedResolution()
        {
            // Apply the selected resolution from the dropdown
            string selectedResolution = resolution.options[resolution.value].text;
            string[] resolutionParts = selectedResolution.Split('x');
            int width = int.Parse(resolutionParts[0].Trim());
            int height = int.Parse(resolutionParts[1].Trim());
            FullScreenMode mode = (FullScreenMode)screenMode.value;
            Screen.SetResolution(width, height, mode);
        }

        public void ShowInGameFps()
        {
            // Show the in-game FPS counter
            inGameFpsCounter.SetActive(true);
            PlayerPrefs.SetInt("etraShowFps", 1);
        }

        public void HideInGameFps()
        {
            // Hide the in-game FPS counter
            inGameFpsCounter.SetActive(false);
            PlayerPrefs.SetInt("etraShowFps", 0);
        }
    }
}