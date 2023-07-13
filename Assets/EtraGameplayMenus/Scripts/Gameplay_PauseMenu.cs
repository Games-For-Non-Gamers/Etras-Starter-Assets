using UnityEngine.SceneManagement;

public class Gameplay_PauseMenu : EtraGameplayMenu
{
    public void backToTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void quitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

}
