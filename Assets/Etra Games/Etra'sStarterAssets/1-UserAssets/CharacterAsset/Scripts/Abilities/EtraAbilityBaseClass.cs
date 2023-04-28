using System;
using UnityEngine;

namespace Etra.StarterAssets.Abilities
{
    public abstract class EtraAbilityBaseClass : MonoBehaviour
    {
        public bool abilityEnabled = true;
        protected EtraCharacterMainController mainController;
        public virtual void abilityStart() { }
        public virtual void abilityUpdate() { }
        public virtual void abilityLateUpdate() { }

        public static explicit operator Type(EtraAbilityBaseClass v)
        {
            throw new NotImplementedException();
        }
    }
}
