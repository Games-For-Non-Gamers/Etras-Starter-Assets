using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    TextMeshProUGUI fpsCounterText;

    void OnEnable()
    {
        fpsCounterText = GetComponent<TextMeshProUGUI>();
        if (UnityEngine.Object.FindObjectOfType<Gameplay_GraphicsMenu>())
        {
            UnityEngine.Object.FindObjectOfType<Gameplay_GraphicsMenu>().showFps.isOn = true;
            EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
        }
    }
    private void OnDisable()
    {
        if (UnityEngine.Object.FindObjectOfType<Gameplay_GraphicsMenu>())
        {
            UnityEngine.Object.FindObjectOfType<Gameplay_GraphicsMenu>().showFps.isOn = false;
            EtraStandardMenuSettingsFunctions.SetGraphicsPlayerPrefs();
        }
    }

    private float deltaTime;
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        int fps = Mathf.RoundToInt(1.0f / deltaTime);
        fpsCounterText.text = "FPS: " + fps;
    }

}
