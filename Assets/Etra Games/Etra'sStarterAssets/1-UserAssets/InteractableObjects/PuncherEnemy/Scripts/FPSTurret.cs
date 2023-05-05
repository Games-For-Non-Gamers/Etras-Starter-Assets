using System.Collections;
using Etra.StarterAssets.Combat;
using UnityEngine;

namespace Etra.StarterAssets.Interactables.Enemies
{
  public class FPSTurret : MonoBehaviour
  {
    public Vector3 startingRotation;
    public float knockbackForce = 150;
    public float spinSpeed = 0.5f;

    [Header("Animation")]
    public GameObject baseSpin;
    public GameObject armSpin;


    //References set by code
    private Animator turretAnimator;
    private GameObject target;
    private bool dieOnce = true;
    private bool playerSpotted = false;
    private HealthSystem healthSystem;

    // Start is called before the first frame update
    void Start()
    {
      turretAnimator = GetComponent<Animator>();
      healthSystem = GetComponent<HealthSystem>();
      healthSystem.OnDeath += () =>
      {
        if (dieOnce)
        {
          dieOnce = false;
          StartCoroutine("die");
        }
      };
      target = EtraCharacterMainController.Instance.modelParent.gameObject.transform.GetChild(0).transform.gameObject;
      startingRotation = baseSpin.transform.rotation.eulerAngles;
      backToIdle();

    }


    bool animStarted = false; //Anim started flag keeps the code from generate a tween every frame
    public virtual void Update()
    {

      if (playerSpotted)
      {
        animStarted = false;
        StopCoroutine(rotateToStart());
        LeanTween.cancel(baseSpin);
        aimAtPlayer();
      }
      else
      {
        if (!animStarted)
        {
          StartCoroutine(rotateToStart());
        }

      }

    }

    // This method is called when the player is detected
    public virtual void playerDetected()
    {
      StopCoroutine("idleTaunts");
      // Set the PlayerSpotted and Attack parameters of the turretAnimator
      turretAnimator.SetBool("PlayerSpotted", true);
      turretAnimator.SetBool("Attack", true);
      // Set the playerSpotted flag to true
      playerSpotted = true;
    }

    // This coroutine will rotate the object back to its starting position
    IEnumerator rotateToStart()
    {
      // Set animStarted flag to true
      animStarted = true;
      // Rotate the object back to its starting position using LeanTween
      LeanTween.rotate(baseSpin, startingRotation, 1);
      // Wait for 1 second
      yield return new WaitForSeconds(1);
      // Set animStarted flag to false
      animStarted = false;
    }

    public void launchPlayer(GameObject player)
    {
      // Add an impulse force to the EtraCharacter to launch the player
      EtraCharacterMainController.Instance.addImpulseForceToEtraCharacter(armSpin.transform.forward + new Vector3(0, 0.3f, 0), knockbackForce);
    }

    public void aimAtPlayer()
    {
      //Base y rotation toward player
      var targetDirection = target.transform.position - baseSpin.transform.position;
      float singleStep = spinSpeed * Time.deltaTime;
      Vector3 newDirection = Vector3.RotateTowards(baseSpin.transform.forward, targetDirection, singleStep, 0.0f);
      Quaternion lookRot = Quaternion.LookRotation(newDirection);
      Quaternion yRotation = new Quaternion(0, lookRot.y, 0, lookRot.w);
      baseSpin.transform.rotation = yRotation;


      //arms x rotation toward player
      var armTargetDirection = target.transform.position - armSpin.transform.position;
      float armSingleStep = spinSpeed * Time.deltaTime;
      Vector3 armNewDirection = Vector3.RotateTowards(armSpin.transform.forward, armTargetDirection, armSingleStep, 0.0f);
      Quaternion armLookRot = Quaternion.LookRotation(armNewDirection);
      var xRotation = new Quaternion(armLookRot.x, 0, 0, armLookRot.w).eulerAngles;
      armSpin.transform.localEulerAngles = xRotation;
    }



    // This variable will be used to prevent the turret from taking damage while it's still in the cooldown period.
    bool isCooling = false;
    // This method will be called when the turret takes damage.
    public void takeDamage(int damage)
    {
      // If the turret is still in cooldown, do nothing.
      if (isCooling)
      {
        return;
      }
      // Reduce the health of the turret by the amount of damage received.
      healthSystem.Damage(damage);



      isCooling = true;
      StartCoroutine("damageAnimation");
    }

    // This coroutine will play the damage animation for a short amount of time.
    IEnumerator damageAnimation()
    {
      turretAnimator.SetBool("Damaged", true);
      yield return new WaitForSeconds(0.01f);
      isCooling = false;
      turretAnimator.SetBool("Damaged", false);
    }

    // This coroutine will play the death animation and destroy the turret object.
    IEnumerator die()
    {
      playerSpotted = false;
      turretAnimator.SetBool("Attack", false);
      turretAnimator.SetBool("Die", true);
      yield return new WaitForSeconds(0.25f);
      //Play particle her in the future
      yield return new WaitForSeconds(1.6f);
      Destroy(gameObject);

    }

    // This method sets the turret back to its idle state.
    public virtual void backToIdle()
    {
      turretAnimator.SetBool("PlayerSpotted", false);
      turretAnimator.SetBool("Attack", false);
      playerSpotted = false;
      StartCoroutine("idleTaunts");

    }

    // This coroutine plays a random taunt animation at random intervals.
    IEnumerator idleTaunts()
    {
      while (true)
      {
        // Wait for a random amount of time between 5 and 11 seconds.
        float idleWait = Random.Range(5, 11);
        yield return new WaitForSeconds(idleWait);
        // Choose a random taunt animation to play.
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


  }
}
