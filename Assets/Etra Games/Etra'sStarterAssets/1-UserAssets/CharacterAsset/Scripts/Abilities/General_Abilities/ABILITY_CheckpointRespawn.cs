using Cinemachine;
using System.Collections;
using UnityEngine;

namespace EtrasStarterAssets
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsage.AbilityTypeFlag.Passive)]

    public class ABILITY_CheckpointRespawn : EtraAbilityBaseClass
    {
        public enum AbilityLockTiming
        {
            LockActiveAbilitiesTillAnimationEnd,
            lockActiveAbilitiesTillTeleport,
            DontLockActiveAbilities
        }

        [Header("Basics")]
        public Vector3 checkpointLocation;
        public Quaternion checkPointRotation;
        [HideInInspector] public bool setRotationToCheckpointRotation;
        [HideInInspector] public bool teleportToGround;
        public bool playAnimation = true;
        public float animationTime = 1;
        public AbilityLockTiming abilityLockTiming = AbilityLockTiming.LockActiveAbilitiesTillAnimationEnd;

        EtraCharacterMainController etraCharacterMainController;
        ABILITY_CameraMovement etraCameraMovement;

        public void Reset()
        {
            transform.parent.GetComponent<EtraCharacterMainController>().setChildObjects(); //string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot, Vector3 localScale
            GameObject spawnedScreenWiper = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("ScreenWiper", this.gameObject.transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.transform, false, Vector3.zero, Quaternion.identity, Vector3.one);
            transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.screenWiper = spawnedScreenWiper;
            transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.setInitialScreenPosition();
        }

        public void Start()
        {
            etraCharacterMainController = EtraCharacterMainController.Instance;
            etraCameraMovement = etraCharacterMainController.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
        }

        bool animating = false;

        public void teleportToCheckpoint()
        {
            if (!abilityEnabled)
            {
                return;
            }

            if (!playAnimation)
            {
                teleportToLocation();
                return;
            }

            if (!animating)
            {
                animating = true;
                StartCoroutine(teleportToCheckpointAnimation());
            }
        }
        IEnumerator teleportToCheckpointAnimation()
        {

            if(abilityLockTiming != AbilityLockTiming.DontLockActiveAbilities){
                etraCharacterMainController.disableAllActiveAbilities();
            }
            etraCharacterMainController.starterAssetCanvas.screenWipe(animationTime);
            yield return new WaitForSeconds(animationTime / 2);
            teleportToLocation();
            if (abilityLockTiming == AbilityLockTiming.lockActiveAbilitiesTillTeleport)
            {  etraCharacterMainController.enableAllActiveAbilities();
            }
            else
            {
                yield return new WaitForSeconds(animationTime/2);
                etraCharacterMainController.enableAllActiveAbilities();
            }
            animating = false;
        }

        Vector3 moveDown = new Vector3(0, -0.01f, 0);
        void teleportToLocation()
        {
            etraCharacterMainController.gameObject.transform.position = checkpointLocation;
            if (setRotationToCheckpointRotation)
            {
                etraCharacterMainController.transform.forward = Vector3.right;
                etraCharacterMainController.gameObject.transform.rotation = checkPointRotation;
                etraCameraMovement.playerCameraRoot.transform.rotation = checkPointRotation;

                etraCameraMovement._cinemachineTargetPitch = checkPointRotation.eulerAngles.x;
                etraCameraMovement._cinemachineTargetYaw = checkPointRotation.eulerAngles.y;
            }

            if (teleportToGround)
            {
                etraCharacterMainController.Grounded = false;
                while (etraCharacterMainController.Grounded != true)
                {
                    etraCharacterMainController.gameObject.transform.position += moveDown;
                    //force a grounded check
                    etraCharacterMainController.GroundedCheck();
                }
            }


        }
    }
}
