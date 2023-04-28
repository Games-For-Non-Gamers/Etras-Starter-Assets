using Etra.StarterAssets.Abilities;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class CheckpointTeleporter : MonoBehaviour
    {
        [Header("Rendering")]
        public bool showInEditor = true;
        public bool showInGame = false;

        private void Reset()
        {
            GetComponent<Renderer>().enabled = showInEditor;
        }
        private void OnValidate()
        {
            GetComponent<Renderer>().enabled = showInEditor;
        }
        private void Start()
        {
            GetComponent<Renderer>().enabled = showInGame;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>())
                {
                    EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>().teleportToCheckpoint();
                }
                else
                {
                    Debug.LogWarning("To Use the Checkpoint Teleporter ABILITY_CheckpointRespawn must be added to your character's ability mannager. ");
                }
            }
        }
    }
}