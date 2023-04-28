using Etra.StarterAssets.Input;
using UnityEngine;

namespace Etra.StarterAssets.Abilities.FirstPerson
{
    [AbilityUsageAttribute(EtraCharacterMainController.GameplayTypeFlags.All)]
    public class ABILITY_Dash : EtraAbilityBaseClass
    {
        public float dashRange = 50.0f;
        public float dashCooldown = 1f;
        public int damageFromDash = 2;

        private float _dashTimeoutDelta = 0;
        private bool cooling;


        StarterAssetsInputs _inputs;

        public override void abilityStart()
        {
            _inputs = GetComponentInParent<StarterAssetsInputs>();
            mainController = GetComponentInParent<EtraCharacterMainController>();
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
                EtraCharacterMainController.Instance.addImpulseForceWithDamageToEtraCharacter(transform.forward, dashRange, damageFromDash, dashCooldown / 2);

                _dashTimeoutDelta = dashCooldown;
                cooling = true;

            }


        }

    }
}
