using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Interactables;
using Etra.StarterAssets.Items;
using UnityEditor;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class NewAbilityOrItemCreatedStarterAssets : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (assetPath.EndsWith(".cs")) // Check if it's a C# script
                {
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                    if (script != null && script.GetClass() != null)
                    {
                        if (script.GetClass().IsSubclassOf(typeof(EtraAbilityBaseClass))) // Check if it's a child of the desired class
                        {
                            PickupAbility[] foundScripts = GameObject.FindObjectsOfType<PickupAbility>(true);
                            foreach (PickupAbility pickupScript in foundScripts)
                            {
                                pickupScript.updateAbilities();
                            }
                        }

                        if (script.GetClass().IsSubclassOf(typeof(EtraFPSUsableItemBaseClass))) // Check if it's a child of the desired class
                        {
                            PickupFPSUsableItem[] foundScripts = GameObject.FindObjectsOfType<PickupFPSUsableItem>(true);
                            foreach (PickupFPSUsableItem pickupScript in foundScripts)
                            {
                                pickupScript.updateItems();
                            }
                        }
                    }
                }
            }
        }

    }
}
