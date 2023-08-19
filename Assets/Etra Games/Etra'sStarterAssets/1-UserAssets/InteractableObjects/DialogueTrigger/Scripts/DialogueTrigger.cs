using Etra.StarterAssets.Source;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class DialogueTrigger : MonoBehaviour
    {
        public static DialogueTrigger prevDialogueTrigger;
        public List<DialogueEntry> dialogueList;
        bool dialogueStarted = false;
        AudioManager audioManager;
        TextMeshProUGUI speakerLabel;
        TextMeshProUGUI dialogueLabel;
        [HideInInspector] public bool canPlayText = true;

        private void Start()
        {
            audioManager = GetComponent<AudioManager>();
            EtraCharacterMainController mainController = FindAnyObjectByType<EtraCharacterMainController>();
            speakerLabel = mainController.starterAssetCanvas.speakerLabel;
            dialogueLabel = mainController.starterAssetCanvas.dialogueLabel;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!dialogueStarted)
                {
                    if (prevDialogueTrigger != null)
                    {
                        prevDialogueTrigger.StopAllCoroutines();
                        prevDialogueTrigger.GetComponent<AudioManager>().stopAllSounds();
                        Destroy(prevDialogueTrigger.gameObject);
                    }

                    prevDialogueTrigger = this;

                    hidePickup();
                    dialogueStarted = true;
                    StartCoroutine(playDialogue());
                    StartCoroutine(destroyObjectTimer());
                }
            }
        }

        void hidePickup()
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }

        IEnumerator playDialogue()
        {

            foreach (DialogueEntry entry in dialogueList)
            {
                if (!canPlayText)
                {
                    yield return null;
                }

                speakerLabel.enabled = false;
                dialogueLabel.enabled = false;

                switch (entry.chosenEvent)
                {
                    case DialogueEntry.DialogueEvents.PlayAudioFromManager:
                        audioManager.Play(entry.sfxName);
                        break;

                    case DialogueEntry.DialogueEvents.UpdateLine:
                        speakerLabel.enabled = true;
                        dialogueLabel.enabled = true;
                        speakerLabel.text = entry.speaker;
                        dialogueLabel.text = entry.dialogueLine;
                        yield return new WaitForSeconds(entry.timeTillNextEvent);
                        break;

                    case DialogueEntry.DialogueEvents.Wait:
                        yield return new WaitForSeconds(entry.timeTillNextEvent);
                        break;

                }

            }
            speakerLabel.enabled = false;
            dialogueLabel.enabled = false;
            yield return null;


        }

        IEnumerator destroyObjectTimer()
        {
            float waitTime = 0;
            foreach (DialogueEntry entry in dialogueList)
            {
                waitTime += entry.timeTillNextEvent;
            }
            waitTime += 5f;

            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }


}