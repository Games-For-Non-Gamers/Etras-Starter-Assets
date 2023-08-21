using Etra.StarterAssets.Source.Interactions;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class Door : MonoBehaviour
    {
        //Enums
        public enum OpenType
        {
            DoorInteract,
            ExternalInteract,
            AutoOpen,
        }
        public enum CloseType
        {
            Never,
            InteractClose,
            ExternalInteract,
            CloseAfterTime,
        }

        [Header("Basics")]
        public float openedRotation = -115f;
        public float timeToOpenAndClose = 0.4f;
        public bool startOpened = false;
        public bool doorUsable = true;
        public OpenType openType = OpenType.DoorInteract;
        public CloseType closeType = CloseType.Never;
        public float timeToCloseAfterTime = 3f;

        [Header("References")]
        public GameObject rotationAxis;
        public ObjectInteraction doorInteracation;
        public DoorAutoOpenHitbox autoOpenHitboxes;
        Coroutine closeDoorRoutine;


        // private
        bool doorOpened = false;
        bool doorMoving = false;
        AudioManager audioManager;

        private void Start()
        {
            //Set starting state and get references
            if (startOpened == false)
            {
                LeanTween.rotateLocal(rotationAxis, Vector3.zero, 0);
                doorOpened = false;
            }
            else
            {
                LeanTween.rotateLocal(rotationAxis, new Vector3(0, 0, openedRotation), 0);
                doorOpened = true;
            }

            audioManager = GetComponent<AudioManager>();
            openTypeSettings();
        }

        private void OnValidate()
        {
            openTypeSettings();
        }

        //Activate the proper interaction objects or scripts.
        void openTypeSettings()
        {
            switch (openType)
            {
                case OpenType.DoorInteract:
                    autoOpenHitboxes.gameObject.SetActive(false);
                    doorInteracation.isInteractable = true;
                    break;

                case OpenType.ExternalInteract:
                    autoOpenHitboxes.gameObject.SetActive(false);
                    doorInteracation.isInteractable = false;
                    break;

                case OpenType.AutoOpen:
                    autoOpenHitboxes.gameObject.SetActive(true);
                    doorInteracation.isInteractable = false;
                    break;
            }
        }

        //This is the function called by external
        public void doorInteract()
        {
            if (doorMoving || !doorUsable)
            {
                return;
            }

            if (!doorOpened)  // Door close
            {
                switch (openType)
                {
                    case OpenType.DoorInteract:
                        StartCoroutine(openDoorAnimation());
                        doorInteracation.isInteractable = false;
                        break;

                    case OpenType.ExternalInteract:
                    case OpenType.AutoOpen:
                        StartCoroutine(openDoorAnimation());
                        break;
                }
            }
            else  // Door open
            {
                switch (closeType)
                {
                    case CloseType.InteractClose:
                        StartCoroutine(closeDoorAnimation());
                        doorInteracation.isInteractable = false;
                        break;

                    case CloseType.CloseAfterTime:
                        // Reset the timer if more open commands are received like with auto open
                        if (closeDoorRoutine != null)
                        {
                            StopCoroutine(closeDoorRoutine);
                        }
                        closeDoorRoutine = StartCoroutine(closeDoorAnimation());
                        break;

                    case CloseType.ExternalInteract:
                        StartCoroutine(closeDoorAnimation());
                        break;

                    case CloseType.Never:
                        return;
                }
            }
        }

        // Animation for opening the door
        IEnumerator openDoorAnimation()
        {
            audioManager.Play("DoorOpen");
            doorMoving = true;
            LeanTween.rotateLocal(rotationAxis, new Vector3(0, 0, openedRotation), timeToOpenAndClose);
            yield return new WaitForSeconds(timeToOpenAndClose);
            doorMoving = false;
            doorOpened = true;

            switch (closeType)
            {
                case CloseType.InteractClose:
                    doorInteracation.isInteractable = true;
                    break;
                case CloseType.CloseAfterTime:
                    if (closeDoorRoutine != null)
                    {
                        StopCoroutine(closeDoorRoutine);
                    }
                    closeDoorRoutine = StartCoroutine(closeDoorAnimation());
                    break;
            }
        }

        // Animation for closing the door
        IEnumerator closeDoorAnimation()
        {
            if (closeType == CloseType.CloseAfterTime)
            {
                yield return new WaitForSeconds(timeToCloseAfterTime);
            }
            audioManager.Play("DoorClose");
            doorMoving = true;
            LeanTween.rotateLocal(rotationAxis, Vector3.zero, timeToOpenAndClose);
            yield return new WaitForSeconds(timeToOpenAndClose);
            doorMoving = false;
            doorOpened = false;
            switch (openType)
            {
                case OpenType.DoorInteract:
                    doorInteracation.isInteractable = true;
                    break;
            }
        }
    }
}
