using UnityEngine.SceneManagement;

namespace Etra.StandardMenus
{
    public class EtraPauseMenu : EtraStandardMenu
    {
        // Go back to the title scene
        public void BackToTitle()
        {
            SceneManager.LoadScene(0);
        }

        // Quit the game
        public void QuitGame()
        {
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