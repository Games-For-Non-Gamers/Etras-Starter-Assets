using Etra.StarterAssets.Interactables.Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets.Source.Combat
{
  public class BulletProjectile : MonoBehaviour
  {
    private List<Collider> colliders = new List<Collider>();
    public int projectileDamage = 1;
    private Rigidbody bulletRigidbody;
    public float bulletSpeed = 10;
    // Start is called before the first frame update
    private void Awake()
    {
      var startScale = transform.localScale;
      bulletRigidbody = GetComponent<Rigidbody>();
      LeanTween.scale(gameObject, Vector3.zero, 0);
      LeanTween.scale(gameObject, startScale, 0.1f);
    }

    private void Start()
    {
      bulletRigidbody.velocity = transform.forward * bulletSpeed;
      Invoke("DestoryAfterTime", 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
      var isDamageableCheck = other.gameObject.GetComponent<IDamageable<int>>();
      if (isDamageableCheck != null)
      {
        isDamageableCheck.TakeDamage(projectileDamage);
        Destroy(gameObject);
      }
    }

    private void OnTriggerExit(Collider other)
    {
      // Remove from list when out of trigger
      colliders.Remove(other);
    }

    void DestoryAfterTime()
    {
      Destroy(gameObject);
    }

    /*
    private void OnDestroy()
    {

        // For every object currently collided with...
        foreach (var x in colliders)
        {
            // ...if the name of the object is the same as the weighted button...
            if (x.name == "ButtonColliderAndCode")
            {
                // ...close the door when projectile destroyed!
                x.GetComponent<Interactable_WeightedButton>().removeObject();
                x.GetComponent<Interactable_WeightedButton>().doorClose();
            }
        }

    }
    */
  }
}
