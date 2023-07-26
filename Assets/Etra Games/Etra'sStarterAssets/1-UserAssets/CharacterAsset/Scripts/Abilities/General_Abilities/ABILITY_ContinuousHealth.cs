using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Etra.StarterAssets.Combat;
using Etra.StarterAssets.Source;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive, typeof(HealthSystem)), RequireComponent(typeof(ABILITY_CheckpointRespawn))]
    class ABILITY_ContinuousHealth : EtraAbilityBaseClass
    {
        [SerializeField, Header("Main variables")]
        AnimationCurve damageCurve;
        [SerializeField]
        float damageCooldownWaitTime, healCooldownWaitTime;
        bool damageCooldown = false, healCooldown = false;
        [SerializeField, Header("Misc"), Tooltip("The index of the damage filter inside the canvas")]
        int siblingIndex = 0;
        int currentStep = 0;
        HealthSystem healthSystem;
        ABILITY_CheckpointRespawn checkpointRespawn;
        Image image;
        float animationTime;
        public override void abilityStart()
        {
            healthSystem = GetComponentInChildren<HealthSystem>();
            healthSystem.OnDamage.AddListener(OnDamage);
            healthSystem.OnHeal.AddListener(OnHeal);
            healthSystem.OnDeath.AddListener(OnDeath);
            healthSystem.OnChange.AddListener(OnChange);
            checkpointRespawn = GetComponent<ABILITY_CheckpointRespawn>();
            animationTime = checkpointRespawn.animationTime * .5f;
            image = EtrasResourceGrabbingFunctions
                .addPrefabFromAssetsByName("DamageFilter", EtraCharacterMainController.Instance.starterAssetCanvas.transform, false, Vector3.zero, new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 0))
                .GetComponent<Image>();
            image.transform.SetSiblingIndex(siblingIndex);
        }

        public void Damage()
        {
            if (damageCooldown || healthSystem.manualDeath) return;

            currentStep = currentStep % damageCurve.keys.Length;
            float damage = damageCurve.keys[currentStep].value;
            currentStep++;
            healthSystem.Damage(damage);
        }

        public void Heal(float health)
        {
            if (healCooldown || healthSystem.manualDeath) return;

            healthSystem.Heal(health);
        }

        void OnDamage(float damageDealt)
        {
            StartCoroutine(DamageCooldown());
        }

        void OnHeal(float healedHealth)
        {
            currentStep = 0;
            StartCoroutine(HealCooldown());
        }

        void OnDeath()
        {
            healthSystem.manualDeath = true;
            checkpointRespawn.teleportToCheckpoint();
            StartCoroutine(Respawn());
        }

        void OnChange(float change)
        {
            float healthLost = healthSystem.maxHealth - healthSystem.health;
            image.color = new Color(image.color.r, image.color.g, image.color.b, healthLost / healthSystem.maxHealth);
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

        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(animationTime);
            healthSystem.Heal(healthSystem.maxHealth, true);
            currentStep = 0;
            healthSystem.manualDeath = false;
        }
    }
}
