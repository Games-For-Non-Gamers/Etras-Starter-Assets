using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Etra.StarterAssets.Abilities.EtraAbilityBaseClass;

namespace Etra.StarterAssets.Source
{
    public class EtrasResourceGrabbingFunctions : MonoBehaviour
    {

        #region getPrefabFromAssetsByName and Overloads
        public static GameObject getPrefabFromResourcesByName(string prefabName)
        {
            GameObject foundObject;
            if (Resources.Load<GameObject>(prefabName) == null)
            {
                Debug.LogError(prefabName + " not found in assets. Please restore the prefab.");
                return null;
            }
            foundObject = Resources.Load<GameObject>(prefabName);
            return foundObject;
        }


        #endregion

        #region addPrefabFromAssetsByName and Overloads
        public static GameObject addPrefabFromResourcesByName(string prefabName)
        {
            return addPrefabFromAssetsByName(prefabName, null);
        }


        public static GameObject addPrefabFromAssetsByName(string prefabName, Transform parent)
        {
            return addPrefabFromAssetsByName(prefabName, parent, false);
        }


        public static GameObject addPrefabFromAssetsByName(string prefabName, Transform parent, bool allowDuplicates)
        {
            if (!allowDuplicates)
            {
                if (GameObject.Find(prefabName))
                {
                    Debug.LogWarning("There is already a " + prefabName + " in the active scene. Please allow duplicates in your addPrefabFromAssetsByName() function call if you wish to allow duplicates of " + prefabName + ".");
                    return null;
                }
            }

            var addedObject = getPrefabFromResourcesByName(prefabName);
            if (addedObject == null) { return null; }


            addedObject = Instantiate(addedObject, Vector3.zero, Quaternion.identity);
#if UNITY_EDITOR
            PrefabUtility.InstantiatePrefab(addedObject);
#endif
            if (!allowDuplicates)
            {
                addedObject.name = prefabName;
            }

            if (parent != null)
            {
                addedObject.transform.SetParent(parent);
            }

            return addedObject;
        }

        public static GameObject addPrefabFromAssetsByName(string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos)
        {
            var addedObject = addPrefabFromAssetsByName(prefabName, parent, allowDuplicates);
            if (addedObject == null) { return null; }
            addedObject.transform.localPosition = localPos;
            return addedObject;
        }

        public static GameObject addPrefabFromAssetsByName(string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot)
        {
            var addedObject = addPrefabFromAssetsByName(prefabName, parent, allowDuplicates, localPos);
            if (addedObject == null) { return null; }
            addedObject.transform.localRotation = localRot;
            return addedObject;
        }

        public static GameObject addPrefabFromAssetsByName(string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot, Vector3 localScale)
        {
            var addedObject = addPrefabFromAssetsByName(prefabName, parent, allowDuplicates, localPos, localRot);
            if (addedObject == null) { return null; }
            addedObject.transform.localScale = localScale;
            return addedObject;
        }

        #endregion

        #region GetComponentFromAssetsByName
        //NOTE This function only works in the Unity Editor. Attempts to make it work in a built version of Unity will not function. Instead use GetComponentFromResourcesByName
        public static Type getComponentFromAssetsByName(string name)
        {
#if UNITY_EDITOR
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name == name)
                        return type;
                }
            }
            Debug.LogError("Component " + name + " not found in any asset folder.");
            return null;
#else
        Debug.LogError("Cannot use the GetComponentFromAssetsByName function outside of the Unity Editor.");
        return null;
#endif
        }


        #endregion

        #region TryGetComponentInChildren
        public static bool TryGetComponentInChildren<T>(Transform parentObject)
        {
            if (parentObject.GetComponentInChildren<T>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion



        public static IEnumerable<EtraAbilityBaseClass> FindAllAbilityScripts<T>() where T : MonoBehaviour
        {
            var targetType = typeof(T);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => targetType.IsAssignableFrom(t) && t != targetType);

                foreach (var type in types)
                {
                    var gameObject = new GameObject("Tempcube");
                    var scriptComponent = gameObject.AddComponent(type);

                    if (scriptComponent is EtraAbilityBaseClass script)
                    {
                        yield return script;
                    }

                    DestroyImmediate(gameObject);
                }
            }
        }


        public static List<AbilityScriptAndNameHolder> GetAllAbilitiesAndSubAbilities()
        {
            //Get all EtraAbilityBaseClass

            List<AbilityScriptAndNameHolder> tempAbilities = new List<AbilityScriptAndNameHolder>();
            tempAbilities = FindAllAbilityScripts<EtraAbilityBaseClass>().Select(x => new AbilityScriptAndNameHolder(x)).ToList();

            List<AbilityScriptAndNameHolder>  abilityAndSubAbilities = new List<AbilityScriptAndNameHolder>();

            foreach (AbilityScriptAndNameHolder a in tempAbilities)
            {
                abilityAndSubAbilities.Add(a);
                EtraAbilityBaseClass e = a.script;

                foreach (subAbilityUnlock sub in a.script.subAbilityUnlocks)
                {
                    abilityAndSubAbilities.Add(new AbilityScriptAndNameHolder(e, sub.subAbilityName));
                }
            }


            //Debug.Log("List elements: " + string.Join(", ", temp));
            return abilityAndSubAbilities;
        }


        public static IEnumerable<EtraFPSUsableItemBaseClass> FindAllFPSItemScripts<T>() where T : MonoBehaviour
        {
            var targetType = typeof(T);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => targetType.IsAssignableFrom(t) && t != targetType);

                foreach (var type in types)
                {
                    var gameObject = new GameObject("Tempcube");
                    var scriptComponent = gameObject.AddComponent(type);

                    if (scriptComponent is EtraFPSUsableItemBaseClass script)
                    {
                        yield return script;
                    }

                    DestroyImmediate(gameObject);
                }
            }
        }


        public static List<ItemScriptAndNameHolder> GetAllItems()
        {
            //Get all EtraAbilityBaseClass

            List<ItemScriptAndNameHolder> tempItems = new List<ItemScriptAndNameHolder>();
            tempItems = FindAllFPSItemScripts<EtraFPSUsableItemBaseClass>().Select(x => new ItemScriptAndNameHolder(x)).ToList();

            //Debug.Log("List elements: " + string.Join(", ", temp));
            return tempItems;
        }


    }

    //Get grandchildren extensions
    public static class ComponentExtensions
    {
        public static T[] GetComponentsInAllChildren<T>(this Component parent, bool includeInactive = false) where T : Component
        {
            return parent.GetComponentsInChildren<T>(includeInactive);
        }

        public static T[] GetComponentsInAllChildren<T>(this GameObject parent, bool includeInactive = false) where T : Component
        {
            return parent.GetComponentsInChildren<T>(includeInactive);
        }

        public static T[] GetComponentsInAllChildren<T>(this Transform parent, bool includeInactive = false) where T : Component
        {
            var components = parent.GetComponents<T>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                components = components.Concat(child.GetComponentsInAllChildren<T>(includeInactive)).ToArray();
            }

            return components;
        }
    }


}