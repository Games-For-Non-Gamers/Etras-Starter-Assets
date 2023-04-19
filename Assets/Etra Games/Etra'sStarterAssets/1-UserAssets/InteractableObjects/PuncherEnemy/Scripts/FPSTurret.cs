using System.Collections;
using UnityEngine;

namespace EtrasStarterAssets{
    public class FPSTurret : Enemy
    {

        public Vector3 startingRotation;
        public bool playerSpotted = false;
        public float playerAim;
        public Animator turretAnimator;
        public GameObject baseSpin;
        public GameObject armSpin;

        public float knockbackForce = 150;

        public float spinSpeed = 5f;

        //public GameObject deathParticle;
        private bool dieOnce = true;
        public GameObject target;
        // Start is called before the first frame update
        void Start()
        {
            turretAnimator = GetComponent<Animator>();
            target = EtraCharacterMainController.Instance.modelParent.gameObject.transform.GetChild(0).transform.gameObject;
            //deathParticle.SetActive(true);
            //deathParticle.GetComponent<ParticleSystem>().Stop();
            //deathParticle.SetActive(false);
            // baseSpin.transform.localEulerAngles = startingRotation;

            startingRotation = baseSpin.transform.rotation.eulerAngles;
            backToIdle();

        }

        bool isCooling = false;
        public void takeDamage(int damage)
        {

            if (isCooling)
            {
                return;
            }

            health = health - damage;
            if (health <= 0)
            {
                if (dieOnce)
                {
                    dieOnce = false;
                    StartCoroutine("die");
                }
            }
            else
            {
                isCooling = true;
                StartCoroutine("damageAnimation");
            }
        }

        IEnumerator damageAnimation()
        {
            turretAnimator.SetBool("Damaged", true);
            yield return new WaitForSeconds(0.01f);
            isCooling = false;
            turretAnimator.SetBool("Damaged", false);
        }

        IEnumerator die()
        {
            playerSpotted = false;
            turretAnimator.SetBool("Attack", false);
            turretAnimator.SetBool("Die", true);
            yield return new WaitForSeconds(0.25f);
            //deathParticle.SetActive(true);
            //deathParticle.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(1.6f);
            Destroy(gameObject);

        }



        public virtual void backToIdle()
        {
            turretAnimator.SetBool("PlayerSpotted", false);
            turretAnimator.SetBool("Attack", false);
            playerSpotted = false;
            StartCoroutine("idleTaunts");

        }

        IEnumerator idleTaunts()
        {
            while (true)
            {
                float idleWait = Random.Range(5, 11);
                yield return new WaitForSeconds(idleWait);
                int whichTaunt = Random.Range(0, 2);

                if (whichTaunt == 0)
                {
                    turretAnimator.SetBool("Taunt1", true);
                    yield return new WaitForSeconds(4.25f);
                    turretAnimator.SetBool("Taunt1", false);
                }
                else if (whichTaunt == 1)
                {
                    turretAnimator.SetBool("Taunt2", true);
                    yield return new WaitForSeconds(2.25f);
                    turretAnimator.SetBool("Taunt2", false);
                }

            }

        }

        public virtual void playerDetected()
        {
            StopCoroutine("idleTaunts");
            turretAnimator.SetBool("PlayerSpotted", true);
            turretAnimator.SetBool("Attack", true);
            playerSpotted = true;
        }
        // Update is called once per frame

        public virtual void Update()
        {

            if (playerSpotted)
            {
                LeanTween.cancel(baseSpin);
                aimAtPlayer();
            }
            else
            {
                LeanTween.rotate(baseSpin, startingRotation, 1);
            }

        }

        public void launchPlayer(GameObject player)
        {
            EtraCharacterMainController.Instance.addImpulseForceToEtraCharacter(armSpin.transform.forward + new Vector3(0, 0.3f, 0), knockbackForce);
            //player.GetComponent<PlayerMovement>().AddImpact(armSpin.transform.forward + new Vector3(0,0.3f,0) , knockbackForce); 
        }

        public void aimAtPlayer()
        {
            //Base y rotation
            Vector3 targetDirection = target.transform.position - baseSpin.transform.position;

            float singleStep = spinSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(baseSpin.transform.forward, targetDirection, singleStep, 0.0f);

            Quaternion lookRot = Quaternion.LookRotation(newDirection);



            Quaternion yRotation = new Quaternion(0, lookRot.y, 0, lookRot.w);
            baseSpin.transform.rotation = yRotation;




            //arms x rotation
            Vector3 armTargetDirection = target.transform.position - armSpin.transform.position;

            float armSingleStep = spinSpeed * Time.deltaTime;
            Vector3 armNewDirection = Vector3.RotateTowards(armSpin.transform.forward, armTargetDirection, armSingleStep, 0.0f);

            Quaternion armLookRot = Quaternion.LookRotation(armNewDirection);

            Vector3 xRotation = new Quaternion(armLookRot.x, 0, 0, armLookRot.w).eulerAngles;
            armSpin.transform.localEulerAngles = xRotation;

        }
    }
}
