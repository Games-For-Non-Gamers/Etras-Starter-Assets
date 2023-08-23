using Etra.StandardMenus;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class ExecutableTitleScreen : MonoBehaviour
    {

        public EtraStandardMenusManager gameSettings;

        // Start is called before the first frame update
        public void GameSettingsPressed()
        {
            gameSettings.PauseInputResults();
            UiSoundManager.I.a.Play("UiClick");
        }

        public string unityAssetLink = "https://youtu.be/VF9EyVxI8o0";
        public void UnityAssetPressed()
        {
            if (!string.IsNullOrEmpty(unityAssetLink))
            {
                Application.OpenURL(unityAssetLink);
            }
            UiSoundManager.I.a.Play("UiClick");
        }

        string youtubeLink = "https://www.youtube.com/channel/UCbivWQmCXFHUpMMAerDMMkQ";
        public void YoutubePressed()
        {
            if (!string.IsNullOrEmpty(youtubeLink))
            {
                Application.OpenURL(youtubeLink);
            }
            UiSoundManager.I.a.Play("UiClick");
        }

        string discordLink = "https://discord.gg/zZdGJQvNvq";
        public void Discord()
        {
            if (!string.IsNullOrEmpty(discordLink))
            {
                Application.OpenURL(discordLink);
            }
            UiSoundManager.I.a.Play("UiClick");
        }

        public void QuitGame()
        {
            UiSoundManager.I.a.Play("UiClick");
#if UNITY_EDITOR
            // In Editor, stop the play mode
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // In standalone build, quit the application
            Application.Quit();
#endif
        }


    }

}
