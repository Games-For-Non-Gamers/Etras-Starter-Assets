using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABILITY_Dash : EtraAbilityBaseClass
{
    public float dashRange = 50.0f;
    public float dashCooldown = 1f;

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
            return;
        }

        if (_dashTimeoutDelta > 0.0f)
        {
            _dashTimeoutDelta -= Time.deltaTime;
        }
        else if (_dashTimeoutDelta < 0.0f && cooling)
        {
            cooling = false;
            _inputs.interact = false;
        }

        if (cooling)
        {
            return;
        }

        if (_inputs.interact)
        {
            EtraCharacterMainController.Instance.addImpulseForceToEtraCharacter(transform.forward, dashRange);
            _dashTimeoutDelta = dashCooldown;
            cooling = true;

        }


    }

}
