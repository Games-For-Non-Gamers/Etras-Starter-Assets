using UnityEngine;

namespace EtrasStarterAssets
{
    public class CheckpointTeleporter : MonoBehaviour
    {

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