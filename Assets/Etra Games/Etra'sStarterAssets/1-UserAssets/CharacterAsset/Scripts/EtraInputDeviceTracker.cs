using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

namespace Etra.StarterAssets
{
    public class EtraInputDeviceTracker : MonoBehaviour
    {
        [HideInInspector] public static EtraInputDeviceTracker Instance;

        [Header("Current UI")]
        public string currentDeviceName = "";
        [HideInInspector] public bool isUsingKeyboard = false;
        [HideInInspector] public bool isUsingGamepad = false;
        private string previousDevice = "";
        private string previousGamepad = "";

        private void Awake()
        {
            //Set up Instance so it can be easily referenced. 
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Instance = this;
            }
        }

        private void OnValidate()
        {
            CheckInputDevice();
        }

        private void OnEnable()
        {
            // Subscribe to input events
            Keyboard.current.onTextInput += OnTextInput;
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            Keyboard.current.onTextInput -= OnTextInput;
        }

        private void Update()
        {
            CheckInputDevice();
            if (previousDevice != currentDeviceName)
            {
                //Run Ui Switch function here.
            }
        }

        private void CheckInputDevice()
        {
            // Update for auto and controller swap
            if (Keyboard.current != null && (Keyboard.current.anyKey.isPressed || Mouse.current.delta.ReadValue().magnitude > 0 || Mouse.current.leftButton.isPressed))
            {
                if (!isUsingKeyboard)
                {
                    isUsingKeyboard = true;
                    isUsingGamepad = false;
                    currentDeviceName = "Keyboard";
                }
            }
            else if (Gamepad.current != null && IsAnyGamepadButtonPressed())
            {
                if (previousGamepad != Gamepad.current.name)
                {
                    previousGamepad = Gamepad.current.name;
                    currentDeviceName = Gamepad.current.name;
                }

                if (!isUsingGamepad)
                {
                    isUsingGamepad = true;
                    isUsingKeyboard = false;
                    currentDeviceName = Gamepad.current.name;
                }
            }
        }

        private void OnTextInput(char character)
        {
            if (!isUsingKeyboard && Keyboard.current.anyKey.isPressed)
            {
                // If not using the keyboard UI and a key is pressed, switch to the keyboard UI
                isUsingKeyboard = true;
                isUsingGamepad = false;
                currentDeviceName = "Keyboard";
            }
            else if (!isUsingGamepad && Gamepad.current != null)
            {
                if (!isUsingKeyboard && IsAnyGamepadButtonPressed())
                {
                    // If not using the gamepad UI and any button on the gamepad is pressed, switch to the gamepad UI
                    isUsingGamepad = true;
                    isUsingKeyboard = false;
                    currentDeviceName = Gamepad.current.displayName;
                }
            }
        }

        private bool IsAnyGamepadButtonPressed()
        {
            foreach (var button in Gamepad.current.allControls)
            {
                if (button is ButtonControl buttonControl && buttonControl.isPressed)
                {
                    return true;
                }
            }
            return false;
        }


    }
}