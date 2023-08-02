using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static Etra.StarterAssets.AbilityUsageAttribute;

namespace Etra.StarterAssets.Abilities
{
    public class EtraAbilityManager : MonoBehaviour
    {
        [Header("The Abilities Will Start(), Update(), and LateUpdate() in The Following Order:")]
        public EtraAbilityBaseClass[] characterAbilityUpdateOrder;


        #region Functions to update The characterAbilityUpdateOrder Array
        public void updateCharacterAbilityArray()
        {
            removeNullAbilitySlots();
            //I understand this is Big O^2 however, it only runs on validate. What's more important is navigation of the final structure (an array) is as fast as possible.
            EtraAbilityBaseClass[] grabbedAbilities;

            if (this == null)
            {
                return;
            }

            if (GetComponent<EtraAbilityBaseClass>() != null)
            {
                grabbedAbilities = GetComponents<EtraAbilityBaseClass>();
            }
            else
            {
                grabbedAbilities = new EtraAbilityBaseClass[0];
            }



            foreach (var ability in grabbedAbilities)
            {
                bool abilityFound = false;

                foreach (var setAbility in characterAbilityUpdateOrder)
                {
                    if (ability.Equals(setAbility))
                    {
                        abilityFound = true;
                    }
                }

                if (!abilityFound)
                {
                    increaseAbilityArrayWithNewElement(ability);
                }
            }
        }

        private void removeNullAbilitySlots()
        {

            if (!FindObjectOfType<EtraAbilityManager>())
            {
                return;
            }

            if (characterAbilityUpdateOrder.Length == 0)
            {
                return;
            }

            bool slotsNeedRemoved = true;
            while (slotsNeedRemoved)
            {
                slotsNeedRemoved = false;
                int elementToPass = 0;
                for (int i = 0; i < characterAbilityUpdateOrder.Length; i++)
                {
                    if (characterAbilityUpdateOrder[i] == null)
                    {
                        slotsNeedRemoved = true;
                        elementToPass = i;
                        i = characterAbilityUpdateOrder.Length;
                    }
                }

                if (slotsNeedRemoved)
                {
                    characterAbilityUpdateOrder = removeElementFromArray(elementToPass);
                }

            }


        }

        private EtraAbilityBaseClass[] removeElementFromArray(int elementToSkip)
        {
            EtraAbilityBaseClass[] shortenedArray = new EtraAbilityBaseClass[characterAbilityUpdateOrder.Length - 1];

            int iterator = 0;
            for (int i = 0; i < characterAbilityUpdateOrder.Length; i++)
            {
                if (i != elementToSkip)
                {
                    shortenedArray[iterator] = characterAbilityUpdateOrder[i];
                    iterator++;
                }
            }

            return shortenedArray;
        }

        private void increaseAbilityArrayWithNewElement(EtraAbilityBaseClass abilityToAdd)
        {
            EtraAbilityBaseClass[] temp = new EtraAbilityBaseClass[characterAbilityUpdateOrder.Length + 1];

            for (int i = 0; i < characterAbilityUpdateOrder.Length; i++)
            {
                temp[i] = characterAbilityUpdateOrder[i];
            }

            temp[characterAbilityUpdateOrder.Length] = abilityToAdd;


            characterAbilityUpdateOrder = temp;
        }
#if UNITY_EDITOR
        EtraAbilityManager()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            ObjectFactory.componentWasAdded += HandleComponentAdded;

            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }
        private void HandleComponentAdded(Component obj)
        {
            updateCharacterAbilityArray();

            if (Application.isPlaying)
            {
                Debug.LogWarning("In the EDITOR PLAY MODE, you can add abilities by adding components to the ability manager.\n" +
                    "However, this will not work in a built game witout an additional step. Everytime you add a component to\n" +
                    "the ability manager, run the updateCharacterAbilityArray() function to add the new item to the correct array.\n" +
                    "Regardless of possibility though, we reccomend adding all your abilities before play and use the abilityEnable\n" +
                    "variable to enable or disable ability scripts.");
            }

        }

        private void OnEditorQuiting()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            EditorApplication.quitting -= OnEditorQuiting;
        }

        private void Reset()
        {
            updateCharacterAbilityArray();
        }

        private void OnValidate()
        {
            updateCharacterAbilityArray();
        }
#endif
        #endregion

        private void Awake()
        {
#if UNITY_EDITOR
            //This is ran to remove any null array elements
            removeNullAbilitySlots();
#endif
        }

        //Run the EtraAbilityBaseClass functions in the order defined above
        private void Start()
        {
            for (int i = 0; i < characterAbilityUpdateOrder.Length; i++)
            {
                characterAbilityUpdateOrder[i].abilityStart();
            }
        }
        private void Update()
        {
            for (int i = 0; i < characterAbilityUpdateOrder.Length; i++)
            {
                characterAbilityUpdateOrder[i].abilityUpdate();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < characterAbilityUpdateOrder.Length; i++)
            {
                characterAbilityUpdateOrder[i].abilityLateUpdate();
            }
        }


        List<EtraAbilityBaseClass> activatedAbilitiesBeforeFreeze;
        public void disableAllActiveAbilities()
        {
            activatedAbilitiesBeforeFreeze = new List<EtraAbilityBaseClass>();

            //Save a list of what abilities are currently active
            for (int i = 0; i < characterAbilityUpdateOrder.Length; i++)
            {
                if (characterAbilityUpdateOrder[i].abilityEnabled)
                {
                    activatedAbilitiesBeforeFreeze.Add(characterAbilityUpdateOrder[i]);
                }
            }

            //Disable all those active abilities
            foreach (var ability in activatedAbilitiesBeforeFreeze)
            {
                var a = ability.GetType().GetCustomAttribute<AbilityUsageAttribute>();
                if (a != null && a.AbilityType == AbilityTypeFlag.Active)
                {
                    ability.abilityEnabled = false;
                }
            }

        }

        public void enableAllActiveAbilities()
        {
            foreach (var ability in activatedAbilitiesBeforeFreeze)
            {
                var a = ability.GetType().GetCustomAttribute<AbilityUsageAttribute>();
                if (a != null && a.AbilityType == AbilityTypeFlag.Active)
                {
                    ability.abilityEnabled = true;
                }
            }
        }
    }
}


