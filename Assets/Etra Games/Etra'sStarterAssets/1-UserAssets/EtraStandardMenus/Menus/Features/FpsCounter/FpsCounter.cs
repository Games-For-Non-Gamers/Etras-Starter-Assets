using TMPro;
using UnityEngine;

namespace Etra.StandardMenus
{
    public class FpsCounter : MonoBehaviour
    {
        private TextMeshProUGUI fpsCounterText;
        private float deltaTime;

        void OnEnable()
        {
            // Get reference to the TextMeshProUGUI component
            fpsCounterText = GetComponent<TextMeshProUGUI>();

            // Check if Gameplay_GraphicsMenu exists in the scene
            EtraGraphicsMenu graphicsMenu = UnityEngine.Object.FindObjectOfType<EtraGraphicsMenu>();
            if (graphicsMenu != null)
            {
                // Enable the showFps toggle and set graphics player prefs
                graphicsMenu.showFps.isOn = true;
                EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
            }
        }

        private void OnDisable()
        {
            // Check if Gameplay_GraphicsMenu exists in the scene
            EtraGraphicsMenu graphicsMenu = UnityEngine.Object.FindObjectOfType<EtraGraphicsMenu>();
            if (graphicsMenu != null)
            {
                // Disable the showFps toggle and set graphics player prefs
                graphicsMenu.showFps.isOn = false;
                EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
            }
        }

        private void Update()
        {
            // Calculate the FPS and update the text
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            int fps = Mathf.RoundToInt(1.0f / deltaTime);
            fpsCounterText.text = "FPS: " + fps;
        }
    }
}