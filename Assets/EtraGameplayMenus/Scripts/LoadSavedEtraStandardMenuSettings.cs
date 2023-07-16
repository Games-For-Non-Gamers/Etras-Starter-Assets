using UnityEngine;

public class LoadSavedEtraStandardMenuSettings : MonoBehaviour
{

    //This class loads the graphics and audio settings set in the EtraStandardMenus
    //The Gameplay settings depend on your game and in Etra's Starter Assets are handled by the LoadSavedEtraStandardGameplayMenuSettings script

    private void Awake()
    {

    }

    private void Start()
    {
        //GAMEPLAY
        //On very first launch of game auto set graphics quality
        if (PlayerPrefs.GetString("unity.player_session_count") == "1")
        {
            EtraStandardMenuSettingsFunctions.AutomaticallySelectQuality();
        }
        EtraStandardMenuSettingsFunctions.LoadGraphicsPlayerPrefs();

        //AUDIO
        EtraStandardMenuSettingsFunctions.LoadAudioPlayerPrefs();
    }



}
