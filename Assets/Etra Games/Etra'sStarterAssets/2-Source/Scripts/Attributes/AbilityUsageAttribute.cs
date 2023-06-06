using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        public AbilityUsageAttribute(GameplayTypeFlags gameplayType, AbilityTypeFlag abilityType, params Type[] requiredComponents)
        {
            GameplayType = gameplayType;
            AbilityType = abilityType;
            List<Type> requiredComponentsList = new List<Type>();
            for (int i = 0; i < requiredComponents.Length; i++)
            {
                if (requiredComponents[i].IsSubclassOf(typeof(Component)))
                    requiredComponentsList.Add(requiredComponents[i]);
            }
            RequiredComponents = requiredComponentsList.ToArray();
        }

        public GameplayTypeFlags GameplayType { get; private set; }
        public AbilityTypeFlag AbilityType { get; private set; }
        public Type[] RequiredComponents { get; private set; } = Array.Empty<Type>();
    }
}
