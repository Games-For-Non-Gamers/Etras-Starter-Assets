using System;
using UnityEngine;
using static Etra.StarterAssets.Abilities.EtraAbilityBaseClass;

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

        //new code

        public virtual void unlockAbility(string abilityName)
        {
            changeAbilityLockState(abilityName, true);
        }

        public virtual void lockAbility(string abilityName)
        {
            changeAbilityLockState(abilityName, false);
        }

        private void changeAbilityLockState(string abilityName, bool state)
        {

            if (abilityName == this.GetType().Name)
            {
                abilityEnabled = state;
            }
            else if (subAbilityUnlocks.Length > 0)
            {
                subAbilityUnlock subAbility = getSubAbility(abilityName);
                if (subAbility != null)
                {
                    subAbility.subAbilityEnabled = state;

                    //if we enable a sub ability the main ability should be enabled
                    abilityEnabled = true;

                    abilityCheckSubAbilityUnlocks();
                }
            }
            else
            {
                Debug.LogWarning("Ability " + abilityName + " not found.");
            }
        }

        private subAbilityUnlock[] _subAbilityUnlocks;
        public virtual subAbilityUnlock[] subAbilityUnlocks
        {
            get { return _subAbilityUnlocks ?? new subAbilityUnlock[0]; }
            set { _subAbilityUnlocks = value; }
        }
        public virtual void abilityCheckSubAbilityUnlocks() { }


        public class subAbilityUnlock{
            public string subAbilityName;
            public bool subAbilityEnabled;

            public subAbilityUnlock(string n, bool e)
            {
                subAbilityName = n;
                subAbilityEnabled = e;
            }
        }

        public subAbilityUnlock getSubAbility(string name)
        {
            subAbilityUnlock returned = null;

            foreach (subAbilityUnlock s in subAbilityUnlocks)
            {
                if (s.subAbilityName == name)
                {
                    returned = s;
                }
            }

            if (returned == null)
            {
                Debug.LogWarning("Sub Ability not found.");
            }
            return returned;
        }

    }
}
