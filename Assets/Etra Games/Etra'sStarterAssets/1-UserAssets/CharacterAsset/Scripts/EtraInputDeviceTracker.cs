using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
        List<IRunsFunctionOnControllerSwitch> objectsWithControllerSwitchFunctions;
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

        private void Start()
        {
            //This is garbage but works for now. Manual drag and drop will be more efficient for later things.
            objectsWithControllerSwitchFunctions = new List<IRunsFunctionOnControllerSwitch>();

            // Find all objects in the scene
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            // Iterate over the objects to find ones implementing the interface
            foreach (GameObject obj in allObjects)
            {
                IRunsFunctionOnControllerSwitch interfaceComponent = obj.GetComponent<IRunsFunctionOnControllerSwitch>();

                if (interfaceComponent != null)
                {
                    objectsWithControllerSwitchFunctions.Add(interfaceComponent);
                }
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
                previousDevice = currentDeviceName;

                foreach (IRunsFunctionOnControllerSwitch obj in objectsWithControllerSwitchFunctions)
                {
                    obj.OnControllerSwitch(currentDeviceName);
                }
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