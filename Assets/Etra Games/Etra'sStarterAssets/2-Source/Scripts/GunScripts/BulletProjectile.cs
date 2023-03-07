using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private List<Collider> colliders = new List<Collider>();

    private Rigidbody bulletRigidbody;
    public float bulletSpeed=10;
    // Start is called before the first frame update
    private void Awake()
    {
        Vector3 startScale = this.transform.localScale;
        bulletRigidbody = GetComponent<Rigidbody>();
        LeanTween.scale(this.gameObject, Vector3.zero, 0);
        LeanTween.scale(this.gameObject, startScale, 0.1f);
    }

    private void Start()
    {
        bulletRigidbody.velocity = transform.forward * bulletSpeed;
        Invoke("DestoryAfterTime", 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add to list when in trigger
        if(!colliders.Contains(other)) { colliders.Add(other); }
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

    private void OnDestroy()
    {
        // For every object currently collided with...
        foreach( var x in colliders )
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

}
