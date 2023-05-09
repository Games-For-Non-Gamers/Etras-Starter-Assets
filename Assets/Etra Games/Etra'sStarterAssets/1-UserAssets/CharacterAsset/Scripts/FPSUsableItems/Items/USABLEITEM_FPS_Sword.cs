using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Interactables.Enemies;
using Etra.StarterAssets.Source.Camera;
using EtrasStarterAssets;
using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets.Items
{
    public class USABLEITEM_FPS_Sword : EtraFPSUsableItemBaseClass
    {
        //Name of Prefab to load and required function
        private string nameOfPrefabToLoadFromAssets = "FPSSwordGroup";
        public override string getNameOfPrefabToLoad() { return nameOfPrefabToLoadFromAssets; }

        [Header("Basics")]
        public float swordRange = 2.5f;
        public float swordKnockback = 5;
        public float swordCooldown = 1f;
        public int swordDamage = 3;
        public Vector2 hitCamShake = new Vector2(1f, 0.1f);

        //Private Variables
        private float _swordTimeoutDelta = 0;
        private bool cooling;

        //References
        StarterAssetsInputs starterAssetsInputs;
        EtraFPSUsableItemManager etraFPSUsableItemManager;
        Transform referenceToSwordTransform;
        Animator swordAnimator;
        ABILITY_CameraMovement camMoveScript;
        AudioManager fpsItemAudioManager;


        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            fpsItemAudioManager = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("FPSItemSfx").GetComponent<AudioManager>();
        }

        public void OnEnable()
        {
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            etraFPSUsableItemManager = GetComponent<EtraFPSUsableItemManager>();
            referenceToSwordTransform = etraFPSUsableItemManager.activeItemPrefab.transform;
            swordAnimator = referenceToSwordTransform.GetComponentInChildren<Animator>();
            camMoveScript = GameObject.Find("EtraAbilityManager").GetComponent<ABILITY_CameraMovement>();
        }

        private void Reset()
        {
            equipSfxName = "SwordEquip";
        }

        GameObject hitObject;
        public void Update()
        {
            if (inputsLocked)
            {
                starterAssetsInputs.shoot = false;
                return;
            }

            if (_swordTimeoutDelta > 0.0f)
            {
                _swordTimeoutDelta -= Time.deltaTime;
            }
            else if (_swordTimeoutDelta < 0.0f && cooling)
            {
                cooling = false;
                starterAssetsInputs.shoot = false;
            }

            if (cooling)
            {
                return;
            }

            if (starterAssetsInputs.shoot)
            {
                swordAnimator.SetTrigger("Swing");
                fpsItemAudioManager.Play("SwordSwing");
                _swordTimeoutDelta = swordCooldown;
                cooling = true;

                if (camMoveScript.objectHit)
                {
                    hitObject = camMoveScript.raycastHit.transform.gameObject;
                    StartCoroutine(addForceMidSwing());
                }


            }
        }

        IEnumerator addForceMidSwing()
        {
            yield return new WaitForSeconds(0.25f);

            if (hitObject != null)
            {
                var isDamageableCheck = hitObject.GetComponent<IDamageable<int>>();
                if (isDamageableCheck != null)
                {
                    isDamageableCheck.TakeDamage(swordDamage);
                }


                if (hitObject != null && hitObject.GetComponent<Rigidbody>() != null)
                {
                    var hitBody = camMoveScript.raycastHit.transform.gameObject.GetComponent<Rigidbody>();
                    if (hitBody.isKinematic == false && Vector3.Distance(Camera.main.transform.position, camMoveScript.pointCharacterIsLookingAt) < swordRange)
                    {
                        var charController = EtraCharacterMainController.Instance.GetComponent<CharacterController>();
                        hitBody.AddForce(charController.transform.forward * swordKnockback, ForceMode.Impulse);
                        CinemachineShake.Instance.ShakeCamera(hitCamShake);
                    }

                }
            }

        }
    }
}
