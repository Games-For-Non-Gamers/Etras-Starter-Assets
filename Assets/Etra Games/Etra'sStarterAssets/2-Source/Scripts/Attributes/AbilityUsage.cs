using System;
using static EtrasStarterAssets.EtraCharacterMainController;

namespace EtrasStarterAssets {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AbilityUsage : Attribute
    {
        public enum AbilityTypeFlag
        {
            Active,
            Passive
        }

        public AbilityUsage(GameplayTypeFlags gameplayType)
        {
            GameplayType = gameplayType;
            AbilityType = AbilityTypeFlag.Active;
        }

        public AbilityUsage(GameplayTypeFlags gameplayType, AbilityTypeFlag abilityType)
        {
            GameplayType = gameplayType;
            AbilityType = abilityType;
        }

        public GameplayTypeFlags GameplayType { get; private set; }
        public AbilityTypeFlag AbilityType { get; private set; }
    }
}