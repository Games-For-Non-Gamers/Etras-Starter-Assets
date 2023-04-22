using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Hitbox : MonoBehaviour, IDamageable<int>
{
    INTERACTABLE_Target target;

    public void TakeDamage(int damage)
    {
        target = GetComponentInParent<INTERACTABLE_Target>();
        target.targetHit();
    }

}
