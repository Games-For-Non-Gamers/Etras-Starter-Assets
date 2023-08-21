using UnityEngine;
using UnityEngine.InputSystem;

namespace Etra.StarterAssets
{
    public class WaitTillButtonsPressed : CustomYieldInstruction
    {
        private InputActionReference[] actions;
        private InputAction[] clonedActions; // Store cloned actions
        private bool[] buttonPressedStates;
        private float[] buttonPressTimestamps;
        private bool allButtonsPressed = false;

        public WaitTillButtonsPressed(InputActionReference[] actions)
        {
            this.actions = actions;
            buttonPressedStates = new bool[actions.Length];
            buttonPressTimestamps = new float[actions.Length];
            clonedActions = new InputAction[actions.Length]; // Create an array to store cloned actions

            // Clone actions and subscribe to button press events using cloned actions
            for (int i = 0; i < actions.Length; i++)
            {
                // Clone the action and assign it to the corresponding slot
                InputAction clonedAction = new InputAction("TempCloneAction" + i, type: InputActionType.Button);

                foreach (InputBinding bind in actions[i].action.bindings)
                {
                    clonedAction.AddBinding(bind);
                }

                clonedActions[i] = clonedAction;
                int index = i; // Capture the index for the lambda function
                clonedActions[i].started += ctx => ButtonStarted(index);
                clonedActions[i].Enable(); // Enable the cloned action
            }
        }

        private void ButtonStarted(int index)
        {
            buttonPressedStates[index] = true;
            buttonPressTimestamps[index] = Time.realtimeSinceStartup;

            // Check if all buttons are pressed within the time window
            float currentTime = Time.realtimeSinceStartup;
            allButtonsPressed = true;
            for (int i = 0; i < buttonPressedStates.Length; i++)
            {
                if (!buttonPressedStates[i] || currentTime - buttonPressTimestamps[i] > 0.2f)
                {
                    allButtonsPressed = false;
                    break;
                }
            }
        }

        public override bool keepWaiting
        {
            get
            {
                return !allButtonsPressed;
            }
        }

        // Clean up the cloned actions when they are no longer needed
        private void OnDestroy()
        {
            for (int i = 0; i < clonedActions.Length; i++)
            {
                clonedActions[i].Disable();
                clonedActions[i].Dispose();
            }
        }
    }
}
