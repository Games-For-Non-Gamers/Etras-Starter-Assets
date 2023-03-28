using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;

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
                Debug.LogWarning("There is already a " + prefabName + " in the active scene. Please allow duplicates in your addPrefabFromAssetsByName() function call if you wish to allow duplicates of " +prefabName+ ".");
                return null;
            }
        }

        GameObject addedObject = getPrefabFromResourcesByName(prefabName);
        if (addedObject == null) { return null;}


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
        GameObject addedObject = addPrefabFromAssetsByName(prefabName, parent, allowDuplicates);
        if (addedObject == null) { return null; }
        addedObject.transform.localPosition= localPos;
        return addedObject;
    }

    public static GameObject addPrefabFromAssetsByName(string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot)
    {
        GameObject addedObject = addPrefabFromAssetsByName(prefabName, parent, allowDuplicates, localPos);
        if (addedObject == null) { return null; }
        addedObject.transform.localRotation = localRot;
        return addedObject;
    }

    public static GameObject addPrefabFromAssetsByName(string prefabName, Transform parent, bool allowDuplicates, Vector3 localPos, Quaternion localRot, Vector3 localScale)
    {
        GameObject addedObject = addPrefabFromAssetsByName(prefabName, parent, allowDuplicates, localPos, localRot);
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
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
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



}
