using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets.Items
{
    public class USABLEITEM_FPS_Blaster : EtraFPSUsableItemBaseClass
    {
        //Name of Prefab to load and required function
        private string nameOfPrefabToLoadFromAssets = "FPSBlasterGroup";
        public override string getNameOfPrefabToLoad() { return nameOfPrefabToLoadFromAssets; }

        //Public variables
        [Header("Basics")]
        public GameObject launchedBullet;
        public float shootingCooldownTime = 0.05f;

        //Timing boolean
        private bool gunCooling = false;

        //References
        StarterAssetsInputs starterAssetsInputs;
        EtraFPSUsableItemManager etraFPSUsableItemManager;
        Transform referenceToBlasterTransform;
        Animator gunAnimator;
        Transform bulletSpawnPos;
        ABILITY_CameraMovement camMoveScript;
        AudioManager fpsItemAudioManager;

        private void Reset()
        {
            // Set example projectile default when this component is added
            equipSfxName = "BlasterEquip";
            launchedBullet = EtrasResourceGrabbingFunctions.getPrefabFromResourcesByName("ExampleProjectile");
        }

        private void Awake()
        {
            enabled = false;
            if (launchedBullet == null)
            {
                launchedBullet = (GameObject)Resources.Load("ExampleProjectile");
            }
        }

        private void Start()
        {
            fpsItemAudioManager = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("FPSItemSfx").GetComponent<AudioManager>();
        }

        public void OnEnable()
        {
            //Set references WHEN THIS SCRIPT IS ENABLED
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            etraFPSUsableItemManager = GetComponent<EtraFPSUsableItemManager>();
            referenceToBlasterTransform = etraFPSUsableItemManager.activeItemPrefab.transform;
            gunAnimator = referenceToBlasterTransform.GetComponentInChildren<Animator>();
            bulletSpawnPos = GameObject.Find("EtraFPSGunFireLocation").transform;
            camMoveScript = GameObject.Find("EtraAbilityManager").GetComponent<ABILITY_CameraMovement>();
        }


        public void Update()
        {
            if (inputsLocked)
            {
                starterAssetsInputs.shoot = false;
                return;
            }

            if (starterAssetsInputs.shoot && !gunCooling)
            {
                gunAnimator.SetTrigger("Shoot");
                fpsItemAudioManager.Play("BlasterShoot");
                var aimDir = (camMoveScript.pointCharacterIsLookingAt - bulletSpawnPos.position).normalized;

                //If gun is in wall, spawn the physical bullets inside of the player camera root (blaster is right at player camera root).
                if (Vector3.Distance(referenceToBlasterTransform.position, camMoveScript.pointCharacterIsLookingAt) < 1.2f) //1.2f is the clipping length
                {
                    Instantiate(launchedBullet.transform, referenceToBlasterTransform.position, Quaternion.LookRotation(aimDir, Vector3.up));
                }
                //Otherwise, spawn them from gun tip.
                else
                {
                    Instantiate(launchedBullet.transform, bulletSpawnPos.transform.position, Quaternion.LookRotation(aimDir, Vector3.up));
                }


                gunCooling = true;
                CinemachineShake.Instance.ShakeCamera(1f, .1f);

                StartCoroutine(GunAnimCooldown());
            }
        }

        IEnumerator GunAnimCooldown()
        {
            yield return new WaitForSeconds(shootingCooldownTime);
            starterAssetsInputs.shoot = false;
            gunCooling = false;
        }


    }
}
