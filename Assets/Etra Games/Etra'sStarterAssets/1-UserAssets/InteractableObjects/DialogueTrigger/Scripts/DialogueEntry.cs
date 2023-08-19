using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    [System.Serializable]
    public class DialogueEntry 
    {
        public enum DialogueEvents
        {
            PlayAudioFromManager,
            UpdateLine,
            Wait,
        }

        public DialogueEvents chosenEvent = DialogueEvents.UpdateLine;

        //Play Audio
        public string sfxName = "DemoDialogue1";

        //Update Line
        public string speaker = "Etra:";
        public string dialogueLine = "This is a demo test line";
        public float timeTillNextEvent = 1.5f; //<---Also for wait
    }
}
