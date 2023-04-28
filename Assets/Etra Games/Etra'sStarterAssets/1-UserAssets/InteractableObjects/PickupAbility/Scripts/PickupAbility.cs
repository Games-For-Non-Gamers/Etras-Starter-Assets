using Etra.StarterAssets.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class PickupAbility : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static List<string> TMPList;
        [HideInInspector] public List<string> abilityShortenedNames;
        [ListToPopup(typeof(PickupAbility), "TMPList")]
        public string Ability_To_Activate;
        private List<Ability> generalAbilities;
        EtraAbilityBaseClass selectedAbility;

        //Set the correct selected ability
        private void Start()
        {
            GetAllAbilities();
            var abilityType = generalAbilities.ElementAt(abilityShortenedNames.IndexOf(Ability_To_Activate)).type;

            //If the ability is not on the player, it cannot be activated or deactivated
            if ((EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(abilityType) == null)
            {
                Debug.LogWarning("PickupAbility.cs cannot activate the " + Ability_To_Activate + " ability on your character because your character does not have the " + Ability_To_Activate + " script attached to its ability manager.");
            }
            else
            {
                selectedAbility = (EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(abilityType);
            }
        }

        //If the player collides with the pickup...
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                //enable the ability and destroy the pickup
                selectedAbility.abilityEnabled = true;
                Destroy(gameObject);
            }

        }


        #region AbilityListDisplay
        public List<string> GetAllAbilities()
        {
            //Get all EtraAbilityBaseClass
            generalAbilities = new List<Ability>();
            generalAbilities = FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();

            List<string> temp = new List<string>();
            foreach (var ability in generalAbilities)
            {
                temp.Add(ability.shortenedName.ToString());
            }
            return temp;
        }

        //Update the list every frame on editor selection "functionally"
        public void OnBeforeSerialize()
        {
            abilityShortenedNames = GetAllAbilities();
            TMPList = abilityShortenedNames;
        }

        public void OnAfterDeserialize()
        {

        }

        //Helper function to find all EtraAbilityBaseClass scripts
        public static IEnumerable<Type> FindAllTypes<T>()
        {
            var type = typeof(T);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => t != type && type.IsAssignableFrom(t));
        }

        //Helper class to find all EtraAbilityBaseClass scripts
        class Ability
        {
            public Ability(Type type)
            {
                this.type = type;
                state = false;
                name = type.Name;
                GenerateName();
            }

            public Type type;
            public string name;
            public string shortenedName;
            public bool state;

            public void GenerateName()
            {
                shortenedName = "";

                string[] splits = type.Name.Split('_');

                if (splits.Length == 2)
                {
                    shortenedName = splits[1];
                }
                else
                {
                    for (int i = 1; i < splits.Length; i++)
                    {
                        shortenedName += splits[i];
                        if (i != splits.Length - 1)
                        {
                            shortenedName += " ";
                        }

                    }
                }

                shortenedName = Regex.Replace(shortenedName, "([a-z])([A-Z])", "$1 $2");
            }
        }
        #endregion
    }
}
