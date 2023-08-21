using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Etra.StarterAssets
{
    [System.Serializable]
    public class GameFreezeEntry
    {
        public enum GameFreezeEvents
        {
            Freeze,
            Popup,
            AdditionalSfx,
            LockMouse,
            UnlockMouse,
            Unfreeze,
            WaitForTime,
            WaitForInput
        }

        public GameFreezeEvents chosenEvent = GameFreezeEvents.Popup;

        //Freeze
        public float backgroundFadeTime = 0.25f;

        //Popup
        public GameObject popupToAdd;
        public Vector2 position;
        public string popupText;
        public float popupTextSize = 19;
        public bool playDefaultAudio = true;
        public bool playDefaultAnimation = true;
        public enum AdvanceType
        {
            Time,
            WaitForInput,
            External
        }
        public AdvanceType advanceType;
        public InputActionReference[] inputsNeededToAdvance;
        public float timeToWait;

        //AdditionalSfx
        public string[] sfxToPlay;
        //Unfreeze

        //WaitTime
        //public float timeToWait;

        //WaitForInput
        //public InputActionReference[][] inputsNeededToAdvance;

    }
}
