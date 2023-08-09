using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Etra.StarterAssets.Combat;
using Etra.StarterAssets.Source;
using UnityEngine.SceneManagement;

namespace Etra.StarterAssets.Abilities
{
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsageAttribute.AbilityTypeFlag.Passive, typeof(HealthSystem))]
    class ABILITY_ContinuousHealth : EtraAbilityBaseClass
    {
        [SerializeField, Header("Main variables")]
        AnimationCurve damageCurve;
        [SerializeField]
        float damageCooldownWaitTime = 0.5f;
        float healCooldownWaitTime = 0;
        bool damageCooldown = false, healCooldown = false;
        int currentStep = 0;
        HealthSystem healthSystem;
        ABILITY_CheckpointRespawn checkpointRespawn;
        [HideInInspector]public GameObject healthFilter;
         Image image;
        float animationTime;


        public void Reset()
        {
            if (this.gameObject.name == "Tempcube") { return; }
            transform.parent.GetComponent<EtraCharacterMainController>().setChildObjects(); //string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot, Vector3 localScale
            
            healthFilter = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("DamageFilter", gameObject.transform.parent.GetComponent<EtraCharacterMainController>().starterAssetCanvas.transform, false, Vector3.zero, Quaternion.identity, Vector3.one);
            damageCurve = new AnimationCurve();
            damageCurve.AddKey(0, 25);
            //Set above the cursor, but behind everything else
            if (healthFilter.gameObject.transform.parent.childCount >1)
            {
                healthFilter.gameObject.transform.SetSiblingIndex(1);
            }
        }

        public override void abilityStart()
        {

            image = healthFilter.GetComponent<Image>();

            healthSystem = GetComponentInChildren<HealthSystem>();
            healthSystem.OnDamage.AddListener(OnDamage);
            healthSystem.OnHeal.AddListener(OnHeal);
            healthSystem.OnDeath.AddListener(OnDeath);
            healthSystem.OnChange.AddListener(OnChange);

            if (GetComponent<ABILITY_CheckpointRespawn>())
            {
                checkpointRespawn = GetComponent<ABILITY_CheckpointRespawn>();
                animationTime = checkpointRespawn.animationTime * .5f;
            }

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
            if (checkpointRespawn)
            {
                healthSystem.manualDeath = true;
                checkpointRespawn.teleportToCheckpoint();
                StartCoroutine(Respawn());
            }
            else
            {
                //Reload current scene 
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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
