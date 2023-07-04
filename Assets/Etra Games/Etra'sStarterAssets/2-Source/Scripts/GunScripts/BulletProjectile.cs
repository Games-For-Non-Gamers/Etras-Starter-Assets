using Etra.StarterAssets.Interactables.Enemies;
using EtrasStarterAssets;
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
        [HideInInspector]
        public GameObject hitObject;
        [HideInInspector]
        public Vector3 hitPoint;


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
            Invoke("DestroyAfterTime", 10f);
        }

        private void OnTriggerEnter(Collider other)
        {
            var isDamageableCheck = other.gameObject.GetComponent<IDamageable<int>>();
            if (isDamageableCheck != null)
            {
                isDamageableCheck.TakeDamage(projectileDamage);
                DestroyBullet();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Remove from list when out of trigger
            colliders.Remove(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.isTrigger)
            {
                GetComponent<AudioManager>().Play("ProjectileBounce");
            }
        }


        private void DestroyAfterTime()
        {
            DestroyBullet();
        }


        private void DestroyBullet()
        {
            MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in comps)
            {
                c.enabled = false;
                Destroy(c);
            }
            Destroy(gameObject);
        }
    }
}