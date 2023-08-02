using Etra.StarterAssets.Input;
using EtrasStarterAssets;
using UnityEngine;

namespace Etra.StarterAssets.Abilities.FirstPerson
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_Dash : EtraAbilityBaseClass
    {
        public float dashRange = 70.0f;
        public float dashCooldown = 1f;
        public int damageFromDash = 2;

        private float _dashTimeoutDelta = 0;
        private bool cooling;

        //references
        private GameObject _mainCamera;
        private AudioManager abilitySoundManager;
        StarterAssetsInputs _inputs;

        public override void abilityStart()
        {
            _inputs = GetComponentInParent<StarterAssetsInputs>();
            mainController = GetComponentInParent<EtraCharacterMainController>();
            //Get sfx manager
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            abilitySoundManager = _mainCamera.transform.Find("AbilitySfx").GetComponent<AudioManager>();
        }

        public override void abilityUpdate()
        {

            if (!enabled || !abilityEnabled)
            {
                _inputs.dash = false;
                return;
            }

            if (_dashTimeoutDelta > 0.0f)
            {
                _dashTimeoutDelta -= Time.deltaTime;
            }
            else if (_dashTimeoutDelta < 0.0f && cooling)
            {
                cooling = false;
                _inputs.dash = false;
            }

            if (cooling)
            {
                return;
            }

            if (_inputs.dash)
            {
                //EtraCharacterMainController.Instance.addImpulseForceToEtraCharacter(transform.forward, dashRange);
                EtraCharacterMainController.Instance.addImpulseForceWithDamageToEtraCharacter(transform.forward, dashRange, damageFromDash, dashCooldown / 2);
                abilitySoundManager.Play("Dash");
                _dashTimeoutDelta = dashCooldown;
                cooling = true;

            }


        }

    }
}
