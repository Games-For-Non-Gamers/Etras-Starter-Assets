using Cinemachine;
using Etra.StarterAssets.Source;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive)]

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
            if (this.gameObject.name == "Tempcube"){return; }

            transform.parent.GetComponent<EtraCharacterMainController>().setChildObjects(); //string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot, Vector3 localScale
            var spawnedScreenWiper = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("ScreenWiper", gameObject.transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.transform, false, Vector3.zero, Quaternion.identity, Vector3.one);
            transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.screenWiper = spawnedScreenWiper;
            transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.setInitialScreenPosition();
        }

        private GameObject _mainCamera;
        private AudioManager abilitySoundManager;

        public override void abilityStart()
        {
            etraCharacterMainController = EtraCharacterMainController.Instance;
            etraCameraMovement = etraCharacterMainController.etraAbilityManager.GetComponent<ABILITY_CameraMovement>();
            //Get sfx manager
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = _mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
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
            abilitySoundManager.Play("ScreenWipe");
            if (abilityLockTiming != AbilityLockTiming.DontLockActiveAbilities)
            {
                etraCharacterMainController.disableAllActiveAbilities();
            }
            etraCharacterMainController.starterAssetCanvas.screenWipe(animationTime);
            yield return new WaitForSeconds(animationTime / 2);
            teleportToLocation();
            if (abilityLockTiming == AbilityLockTiming.lockActiveAbilitiesTillTeleport)
            {
                etraCharacterMainController.enableAllActiveAbilities();
            }
            else
            {
                yield return new WaitForSeconds(animationTime / 2);
                etraCharacterMainController.enableAllActiveAbilities();
            }
            animating = false;
        }

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
                etraCharacterMainController.teleportToGround();
            }


        }
    }
}
