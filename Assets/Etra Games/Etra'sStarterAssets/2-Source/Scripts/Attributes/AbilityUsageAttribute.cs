using System;
using static Etra.StarterAssets.EtraCharacterMainController;

namespace Etra.StarterAssets 
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AbilityUsageAttribute : Attribute
    {
        public enum AbilityTypeFlag
        {
            Active,
            Passive
        }

        public AbilityUsageAttribute(GameplayTypeFlags gameplayType)
        {
            GameplayType = gameplayType;
            AbilityType = AbilityTypeFlag.Active;
        }

        public AbilityUsageAttribute(GameplayTypeFlags gameplayType, AbilityTypeFlag abilityType)
        {
            GameplayType = gameplayType;
            AbilityType = abilityType;
        }

        public GameplayTypeFlags GameplayType { get; private set; }
        public AbilityTypeFlag AbilityType { get; private set; }
    }
}