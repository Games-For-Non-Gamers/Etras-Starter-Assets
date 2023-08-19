using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Etra.StarterAssets
{
    public class GameFreezePopup : MonoBehaviour
    {
        public InputActionReference action;

        // Update is called once per frame
        void Update()
        {
            // Check if the action button is pressed
            if (action.action.ReadValue<float>() > 0.0f)
            {
                Debug.Log("Action button is pressed!");
            }
        }
    }
}
