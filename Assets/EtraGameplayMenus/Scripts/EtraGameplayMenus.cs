using Etra.StarterAssets;
using Etra.StarterAssets.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;


public class EtraGameplayMenus : MonoBehaviour
{

    [Header("Options")]
    public bool canPause = true;
    public bool showBackground = true;

    [Header("References")]
    public GameObject background;
    public GameObject pauseMenu;
    public GameObject gameplayMenu;
    public GameObject graphicsMenu;
    public GameObject audioMenu;

    bool gamePaused = false;
    GameObject currentlyActiveMenu;

    //private references
    EventSystem eventSystem;


    #region PauseInput
    //************** INPUTS/CONTROLLER OR KEYBOARD SELECTION FOR PAUSE EVENT************** 
    StarterAssetsInputs _inputs;
    public PlayerInput playerInput;
    private void Start()
    {
        _inputs = FindObjectOfType<StarterAssetsInputs>();
        eventSystem = GetComponentInChildren<EventSystem>();

        if (EtraCharacterMainController.Instance)
        {
            EtraCharacterMainController.Instance.disableAllActiveAbilities();

            if (playerInput == null)
            {
                playerInput = EtraCharacterMainController.Instance.GetComponent<PlayerInput>();
            }
        }
        unpauseGame();
        closeMenu(gameplayMenu);
        closeMenu(graphicsMenu);
        closeMenu(audioMenu);
        disableBackground();
    }


    private InputAction keyboardEscape;
    private InputAction gamepadStart;
    void OnEnable()
    {
        // Enable the key action
        keyboardEscape = new InputAction("KeyboardEscape", binding: "<Keyboard>/escape");
        keyboardEscape.Enable();
        gamepadStart = new InputAction("GamepadStart", binding: "<Gamepad>/start");
        gamepadStart.Enable();
    }

    void OnDisable()
    {
        keyboardEscape.Disable();
        keyboardEscape.Dispose();
        gamepadStart.Disable();
        gamepadStart.Dispose();
    }

    GameObject savedSelectedObject = null;
    void Update()
    {
        if (canPause)
        {
            if (keyboardEscape.triggered || gamepadStart.triggered)
            {
                eventSystem.SetSelectedGameObject(null);
                pauseOrUnpause();
            }

        }

        //If the game is paused check if selection should be with mouse/keyboard or gamepad
        if (gamePaused)
        {

            if (EtraInputDeviceTracker.Instance.isUsingKeyboard)
            {
                if (eventSystem.currentSelectedGameObject != null)
                {
                    savedSelectedObject = eventSystem.currentSelectedGameObject;
                }

                eventSystem.SetSelectedGameObject(null);
                Cursor.visible = true;
            }
            else if (EtraInputDeviceTracker.Instance.isUsingGamepad)
            {
                Cursor.visible = false;
                if (savedSelectedObject != null && savedSelectedObject.gameObject.activeInHierarchy)
                {
                    eventSystem.SetSelectedGameObject(savedSelectedObject);
                }
                else if (currentlyActiveMenu != null)
                {
                    eventSystem.SetSelectedGameObject(currentlyActiveMenu.GetComponent<EtraGameplayMenu>().firstSelectedObject.gameObject);
                }
            }


        }
    }


    //************************************************************* 
    #endregion

    #region BackgroundImage
    void enableBackground()
    {
        if (showBackground)
        {
            background.gameObject.SetActive(true);
            background.GetComponent<Image>().enabled = true;
        }
    }

    void disableBackground()
    {
        background.gameObject.SetActive(false);
        background.GetComponent<Image>().enabled = false;
    }
    #endregion

    #region MenuFunctions
    void openMenu(GameObject menu)
    {

        EtraGameplayMenu gameplayMenu = menu.GetComponent<EtraGameplayMenu>();

        if (currentlyActiveMenu != null)
        {
            closeMenu(currentlyActiveMenu);
        }
        else
        {
            closeMenu(pauseMenu);
        }

        menu.SetActive(true);
        currentlyActiveMenu = menu;

        //If we are using gamepad select the new higlighted button
        if (EtraInputDeviceTracker.Instance.isUsingGamepad)
        {
            eventSystem.SetSelectedGameObject(gameplayMenu.firstSelectedObject.gameObject);
            Cursor.visible = false;
        }
        else if (EtraInputDeviceTracker.Instance.isUsingKeyboard)
        {
            eventSystem.SetSelectedGameObject(null);
            Cursor.visible = true;
        }
    }

    void closeMenu(GameObject menu)
    {
        menu.gameObject.SetActive(false);
    }

    #endregion

    #region PauseMenu
    void pauseOrUnpause()
    {
        if (!canPause)
        {
            return;
        }

        if (!gamePaused)
        {
            pauseGame();
        } 
        else if (gamePaused)
        {
            unpauseGame();
        }
    }

    void pauseGame()
    {
        
        enableBackground();
        openMenu(pauseMenu);

        _inputs.SetCursorState(false); //Free the cursor

        if (EtraCharacterMainController.Instance)
        {
            EtraCharacterMainController.Instance.disableAllActiveAbilities();
        }
        Time.timeScale = 0;
        gamePaused = true;
        playerInput.SwitchCurrentActionMap("UI");
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }

    public void unpauseGame()
    {
        disableBackground();
        if(currentlyActiveMenu != null)
        {
            closeMenu(currentlyActiveMenu);
        }
        else
        {
            closeMenu(pauseMenu);
        }

        _inputs.SetCursorState(true); //lock the cursor
        Time.timeScale = 1;
        if (EtraCharacterMainController.Instance)
        {
            EtraCharacterMainController.Instance.enableAllActiveAbilities();
        }
        gamePaused = false;

        playerInput.SwitchCurrentActionMap("Player");
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    }

    public void backToPauseMenu()
    {
        openMenu(pauseMenu);
    }

    #endregion


    #region OtherMenus
    public void openGameplayMenu()
    {
        openMenu(gameplayMenu);
    }

    public void openGraphicsMenu()
    {
        openMenu(graphicsMenu);
    }

    public void openAudioMenu()
    {
        openMenu(audioMenu);
    }

    #endregion

}


