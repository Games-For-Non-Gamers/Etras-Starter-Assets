using System;
using System.Collections;
using UnityEngine;
using Etra.StarterAssets;
using Etra.StarterAssets.Combat;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive), RequireComponent(typeof(ABILITY_CheckpointRespawn))]
    class ABILITY_ContinuousHealth : EtraAbilityBaseClass
    {
        [SerializeField]
        float damageCooldownWaitTime, healCooldownWaitTime;
        bool damageCooldown = false, healCooldown = false;
        HealthSystem healthSystem;
        ABILITY_CheckpointRespawn checkpointRespawn;
        public override void abilityStart()
        {
            healthSystem = GetComponentInChildren<HealthSystem>();
            checkpointRespawn = GetComponent<ABILITY_CheckpointRespawn>();
            healthSystem.OnDamage.AddListener(OnDamage);
            healthSystem.OnHeal.AddListener(OnHeal);
            healthSystem.OnDeath.AddListener(checkpointRespawn.teleportToCheckpoint);
        }
        public void Damage(float damage)
        {
            if (damageCooldown) return;

            healthSystem.Damage(damage);
        }

        public void Heal(float health)
        {
            if (healCooldown) return;

            healthSystem.Heal(health);
        }

        void OnDamage(float damageDealt)
        {
            StartCoroutine(DamageCooldown());
        }

        void OnHeal(float healedHealth)
        {
            StartCoroutine(HealCooldown());
        }

        IEnumerator DamageCooldown()
        {
            damageCooldown = true;
            yield return new WaitForSeconds(damageCooldownWaitTime);
            damageCooldown = false;
        }

        IEnumerator HealCooldown()
        {
            healCooldown = true;
            yield return new WaitForSeconds(healCooldownWaitTime);
            healCooldown = false;
        }
    }
}
