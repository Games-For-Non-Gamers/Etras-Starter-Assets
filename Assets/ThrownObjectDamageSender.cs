using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownObjectDamageSender : MonoBehaviour
{
    public int objectDamage = 1;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        IDamageable<int> isDamageableCheck = collision.gameObject.GetComponent<IDamageable<int>>();
        if (isDamageableCheck != null)
        {
            isDamageableCheck.TakeDamage(objectDamage);
        }

        Destroy(this);
    }
}
