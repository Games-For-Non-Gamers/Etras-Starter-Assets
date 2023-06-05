using System;
using UnityEngine;
using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Combat;
using System.Collections;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive)]
    class ABILITY_ContinuousHealth : EtraAbilityBaseClass
    {
        [SerializeField]
        float cooldownWaitTime;
        bool cooldown = false;
        HealthSystem healthSystem;
        public override void abilityStart()
        {
            healthSystem = GetComponentInChildren<HealthSystem>();
            healthSystem.OnDamage.AddListener(OnDamage);
        }
        public void Damage(float damage)
        {
            if (cooldown) return;

            healthSystem.Damage(damage);
        }

        void OnDamage(float damageDealt)
        {
            StartCoroutine(Cooldown());
        }

        IEnumerator Cooldown()
        {
            cooldown = true;
            yield return new WaitForSeconds(cooldownWaitTime);
            cooldown = false;
        }
    }
}
