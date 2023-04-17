using UnityEngine;

namespace EtrasStarterAssets
{
    public class CheckpointUpdater : MonoBehaviour
    {

        public bool setRotationToCheckpointRotation = true;
        public bool teleportToGround = false;

        ABILITY_CheckpointRespawn checkPointRespawnScript;
        private void Start()
        {
            if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>() == null)
            {
                Debug.LogError("The Checkpoint Updater or Checkpoint Teleporters will not work unless your character has the ABILITY_CheckpointRespawn script.");
                return;
            }
            checkPointRespawnScript = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                checkPointRespawnScript.checkpointLocation = this.gameObject.transform.position;
                checkPointRespawnScript.checkPointRotation = this.gameObject.transform.rotation;
                checkPointRespawnScript.setRotationToCheckpointRotation = setRotationToCheckpointRotation;
                checkPointRespawnScript.teleportToGround = teleportToGround;
            }
        }
    }
}
