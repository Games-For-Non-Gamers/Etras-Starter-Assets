using System.Collections;
using UnityEngine;

namespace EtrasStarterAssets{
    public class USABLEITEM_FPS_Sword : EtraFPSUsableItemBaseClass
    {
        //Name of Prefab to load and required function
        private string nameOfPrefabToLoadFromAssets = "FPSSwordGroup";
        public override string getNameOfPrefabToLoad() { return nameOfPrefabToLoadFromAssets; }

        [Header("Basics")]
        public float swordRange = 2.5f;
        public float swordKnockback = 5;
        public float swordCooldown = 1f;
        public int swordDamage =3;
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


        private void Awake()
        {
            this.enabled = false;
        }

        public void OnEnable()
        {
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
            etraFPSUsableItemManager = GetComponent<EtraFPSUsableItemManager>();
            referenceToSwordTransform = etraFPSUsableItemManager.activeItemPrefab.transform;
            swordAnimator = referenceToSwordTransform.GetComponentInChildren<Animator>();
            camMoveScript = GameObject.Find("EtraAbilityManager").GetComponent<ABILITY_CameraMovement>();
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
                IDamageable<int> isDamageableCheck = hitObject.GetComponent<IDamageable<int>>();
                if (isDamageableCheck != null)
                {
                    isDamageableCheck.TakeDamage(swordDamage);
                }


                if (hitObject != null && hitObject.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody hitBody = camMoveScript.raycastHit.transform.gameObject.GetComponent<Rigidbody>();
                    if (hitBody.isKinematic == false && Vector3.Distance(Camera.main.transform.position, camMoveScript.pointCharacterIsLookingAt) < swordRange)
                    {
                        CharacterController charController = EtraCharacterMainController.Instance.GetComponent<CharacterController>();
                        hitBody.AddForce(charController.transform.forward * swordKnockback, ForceMode.Impulse);
                        CinemachineShake.Instance.ShakeCamera(hitCamShake);
                    }

                }
            }

        }
    }
}
