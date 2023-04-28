using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Etra.StarterAssets.Source.Editor
{
    public class EtraStarterAssetSetup : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var inPackages = importedAssets.Any(path => path.StartsWith("Packages/")) ||
                deletedAssets.Any(path => path.StartsWith("Packages/")) ||
                movedAssets.Any(path => path.StartsWith("Packages/")) ||
                movedFromAssetPaths.Any(path => path.StartsWith("Packages/"));

            if (inPackages)
            {
                InitializeOnLoad();
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            addLayer("Player");
            addLayer("EtraFPSUsableItem");

        }



        public static void addTag(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");

            string s = tagName;

            bool found = false;
            int amountOfTags = 0;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                var t = tagsProp.GetArrayElementAtIndex(i);
                amountOfTags++;
                if (t.stringValue.Equals(s)) { found = true; break; }
            }


            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(amountOfTags);
                var n = tagsProp.GetArrayElementAtIndex(amountOfTags);
                n.stringValue = s;
                Debug.Log("Tag:" + tagName + " added by EtraStarterAssetSetup.cs");
            }

            tagManager.ApplyModifiedProperties();

        }


        public static void addLayer(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layersProp = tagManager.FindProperty("layers");

            string s = layerName;

            // First check if it is not already present
            bool found = false;
            int amountOfLayers = 0;
            for (int i = 0; i < layersProp.arraySize; i++)
            {
                var t = layersProp.GetArrayElementAtIndex(i);
                if (!t.stringValue.Equals(""))
                {
                    amountOfLayers++;
                }

                if (t.stringValue.Equals(s)) { found = true; break; }
            }


            if (amountOfLayers >= 32)
            {
                Debug.LogError("Cannot add additional layer " + layerName + " because max layers are reached. " +
                    "Etra's Starter assets needs the additional layer " + layerName + " in order for some features to function.");
                return;
            }
            // if not found, add it
            if (!found)
            {
                bool openLayerFound = false;

                while (openLayerFound == false)
                {
                    var t = layersProp.GetArrayElementAtIndex(amountOfLayers);

                    if (!t.stringValue.Equals(""))
                    {
                        amountOfLayers--;
                    }
                    else
                    {
                        openLayerFound = true;
                    }

                }

                layersProp.InsertArrayElementAtIndex(amountOfLayers);
                var n = layersProp.GetArrayElementAtIndex(amountOfLayers);
                n.stringValue = s;
                Debug.Log("Layer:" + layerName + " added by EtraStarterAssetSetup.cs");
            }


            tagManager.ApplyModifiedProperties();


        }
    }
}
