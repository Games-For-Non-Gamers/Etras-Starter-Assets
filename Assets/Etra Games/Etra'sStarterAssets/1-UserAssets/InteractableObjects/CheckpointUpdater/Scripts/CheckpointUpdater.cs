using Etra.StarterAssets.Abilities;
using UnityEditor;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class CheckpointUpdater : MonoBehaviour
    {
        [Header("Basics")]
        public bool setRotationToCheckpointRotation = true;
        public bool teleportToGround = false;

        [Header("Rendering")]
        public bool showInEditor = true;
        public bool showInGame = false;

        [HideInInspector] public CheckpointUpdaterTeleportLocation teleportLocation;
        [HideInInspector] public CheckpointUpdaterArrowMeshHolder arrowMeshHolder;
        [HideInInspector] public CheckpointUpdaterHitbox checkpointHitbox;
        ABILITY_CheckpointRespawn checkPointRespawnScript;

        private void Reset()
        {
            getScripts();
        }
        private void Awake()
        {
            getScripts();
        }
        private void OnValidate()
        {
            getScripts();

            if (showInEditor)
            {
                showRenderers();
            }
            else
            {
                hideRenderers();
            }

        }
        private void getScripts()
        {
            teleportLocation = GetComponentInChildren<CheckpointUpdaterTeleportLocation>();
            arrowMeshHolder = GetComponentInChildren<CheckpointUpdaterArrowMeshHolder>();
            checkpointHitbox = GetComponentInChildren<CheckpointUpdaterHitbox>();
        }

        private void hideRenderers()
        {
            checkpointHitbox.meshRenderer.enabled = false;
            teleportLocation.meshRenderer.enabled = false;
            arrowMeshHolder.disableMeshes();
        }

        private void showRenderers()
        {
            checkpointHitbox.meshRenderer.enabled = true;
            teleportLocation.meshRenderer.enabled = true;
            if (setRotationToCheckpointRotation)
            {
                arrowMeshHolder.enableMeshes();
            }
            else
            {
                arrowMeshHolder.disableMeshes();
            }
        }

        private void Start()
        {
            if (showInGame)
            {
                showRenderers();
            }
            else
            {
                hideRenderers();
            }

            if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>() == null)
            {
                Debug.LogError("The Checkpoint Updater or Checkpoint Teleporters will not work unless your character has the ABILITY_CheckpointRespawn script.");
                return;
            }
            checkPointRespawnScript = EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>();


        }


        public void hitboxHitByPlayer()
        {
            checkPointRespawnScript.checkpointLocation = teleportLocation.transform.position;
            checkPointRespawnScript.checkPointRotation = teleportLocation.transform.rotation;
            checkPointRespawnScript.setRotationToCheckpointRotation = setRotationToCheckpointRotation;
            checkPointRespawnScript.teleportToGround = teleportToGround;
        }


    }
}
