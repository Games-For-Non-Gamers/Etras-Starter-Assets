using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTeleporter : MonoBehaviour
{
    public GameObject objectToTeleport; // The object to teleport
    public Transform teleportLocation; // The location to teleport the object to
    public bool rigidbody = false; // Wether to use Rigidbody.position or Transform.position
    public bool rotationChange = true; // Wether or not to also change the object's rotation

    public void Teleport()
    {
        // Teleport the object
        if (rigidbody)
        {
            objectToTeleport.GetComponent<Rigidbody>().position = teleportLocation.position;
            if (rotationChange)
            {
                objectToTeleport.GetComponent<Rigidbody>().rotation = teleportLocation.rotation;
                // Set angular velocity to 0
                objectToTeleport.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            // Set velocity to 0
            objectToTeleport.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            objectToTeleport.transform.position = teleportLocation.position;
            if (rotationChange)
            {
                objectToTeleport.transform.rotation = teleportLocation.rotation;
            }
        }
    }
}