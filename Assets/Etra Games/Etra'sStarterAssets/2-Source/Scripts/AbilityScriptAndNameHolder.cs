using Etra.StarterAssets.Abilities;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class AbilityScriptAndNameHolder
    {
        public EtraAbilityBaseClass script;
        public string name;
        public string shortenedName;
        public bool state;

        //For general ability
        public AbilityScriptAndNameHolder(EtraAbilityBaseClass script)
        {
            this.script = script;
            state = false;
            name = script.GetType().Name;
            GenerateName();
        }

        //For sub ability
        public AbilityScriptAndNameHolder(EtraAbilityBaseClass script, string n)
        {
            this.script = script;
            state = false;
            name = script.GetType().Name;
            GenerateName();
            name = n;
            shortenedName += ": ";
            shortenedName += n;
        }


        public void GenerateName()
        {
            shortenedName = "";

            string[] splits = script.GetType().Name.Split('_');

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
}
