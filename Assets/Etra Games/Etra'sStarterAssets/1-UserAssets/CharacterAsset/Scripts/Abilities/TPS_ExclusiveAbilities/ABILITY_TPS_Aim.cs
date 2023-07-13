using Cinemachine;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source;
using Etra.StarterAssets.Source.Camera;
using UnityEngine;

namespace Etra.StarterAssets.Abilities.ThirdPerson
{
    [RequireComponent(typeof(ABILITY_CameraMovement))]

    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.ThirdPerson)]
    public class ABILITY_TPS_Aim : EtraAbilityBaseClass
    {
        [Header("Basics")]
        [SerializeField] private float aimSensitivity = 0.5f;

        //References
        [HideInInspector] public CinemachineVirtualCamera aimVirtualCamera; // aim camera
        ABILITY_CameraMovement camMoveScript;
        ABILITY_CharacterMovement charMoveScript;
        private StarterAssetsInputs starterAssetsInputs;


        //Load third person aim camera and add it to the starter asset group
        public void Reset()
        {
            if (this.gameObject.name == "Tempcube") { return; }
            var aimCam = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("Etra'sStarterAssetsThirdPersonAimCamera", GameObject.FindGameObjectWithTag("Player").transform.parent);
            aimVirtualCamera = aimCam.GetComponent<CinemachineVirtualCamera>();
            aimCam.AddComponent<EtraCharacterAssetCamera>();
        }

        public override void abilityStart()
        {
            //Set references
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            charMoveScript = GetComponent<ABILITY_CharacterMovement>();
            camMoveScript = GetComponent<ABILITY_CameraMovement>();
        }
        public override void abilityUpdate()
        {
            //If ability disabled, then return
            if (!abilityEnabled)
            {
                return;
            }


            if (starterAssetsInputs.aim)
            {
                //If aim pressed activate aim camera and change sensitivity variable
                aimVirtualCamera.gameObject.SetActive(true);

                camMoveScript.aimSensitivity = aimSensitivity * camMoveScript.updateUsedCameraSensitivity(); ;

                camMoveScript.usedCameraSensitivity = 1; // nullify cam sensitivity and just go by aim sensitivity


                //Rotate character toward look direction
                var worldAimTarget = camMoveScript.pointCharacterIsLookingAt;
                worldAimTarget.y = transform.parent.position.y;
                var aimDirection = (worldAimTarget - transform.parent.position).normalized;
                transform.parent.forward = Vector3.Lerp(transform.parent.forward, aimDirection, Time.deltaTime * 20f);
                charMoveScript.rotateTowardMoveDirection = false;
            }
            else
            {
                //If aim let go, deactivate aim camera and change sensitivity variable
                charMoveScript.rotateTowardMoveDirection = true;
                camMoveScript.usedCameraSensitivity = camMoveScript.updateUsedCameraSensitivity();
                camMoveScript.aimSensitivity = 1;  // nullify aim sensitivity and just go by base cam sensitivity
                aimVirtualCamera.gameObject.SetActive(false);
            }
        }

    }
}
