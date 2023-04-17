using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointUpdaterTeleportLocation : MonoBehaviour
{
    public MeshRenderer renderer;

    private void Reset()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    private void OnValidate()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }
}
