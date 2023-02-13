using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_CubeButton : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject door = transform.parent.parent.GetChild(0).gameObject;
        SciFiDoor doorScript = door.GetComponent<SciFiDoor>();

        if (other.gameObject.tag == "Player")
        {
            doorScript.SetOpened(true);
        }
        
        else if (other.GetComponent<Rigidbody>() != null)
        {
            doorScript.SetOpened(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject door = transform.parent.parent.GetChild(0).gameObject;
        SciFiDoor doorScript = door.GetComponent<SciFiDoor>();

        if (other.gameObject.tag == "Player")
        {
            doorScript.SetOpened(false);
        }

        else if (other.GetComponent<Rigidbody>() != null)
        {
            doorScript.SetOpened(false);
        }
    }
}
