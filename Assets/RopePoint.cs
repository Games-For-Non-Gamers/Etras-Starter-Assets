using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour, ISerializationCallbackReceiver
{
     public INTERACTABLE_Target target;
    private void Reset()
    {
        target = GetComponentInParent<INTERACTABLE_Target>();
        OnBeforeSerialize();
    }
    public void OnBeforeSerialize()
    {
        if (target != null)
        {
            target.OnBeforeSerialize();
        }
    }

    public void OnAfterDeserialize()
    {

    }
}
