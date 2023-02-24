using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_CubeButton : MonoBehaviour
{
    private int numObjects = 0;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(numObjects);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject door = transform.parent.parent.GetChild(0).gameObject;
        SciFiDoor doorScript = door.GetComponent<SciFiDoor>();

        if (other.gameObject.tag == "Player")
        {
            numObjects++;
            if (numObjects == 1) doorScript.SetOpened(true);
        }
        
        else if (other.GetComponent<Rigidbody>() != null && numObjects == 0)
        {
            numObjects++;
            if (numObjects == 1) doorScript.SetOpened(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject door = transform.parent.parent.GetChild(0).gameObject;
        SciFiDoor doorScript = door.GetComponent<SciFiDoor>();
        if (numObjects >= 1) numObjects--;

        if (other.gameObject.tag == "Player" && numObjects == 0)
        {
            doorScript.SetOpened(false);
        }

        else if (other.GetComponent<Rigidbody>() != null && numObjects == 0)
        {
            doorScript.SetOpened(false);
        }
    }
}
