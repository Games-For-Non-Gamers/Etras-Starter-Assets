using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_CubeButton : MonoBehaviour
{
    public SciFiDoor door;
    private Transform buttonMain;
    Vector3 buttonStartPos = new Vector3(0, -0.94f, 0);
    Vector3 buttonEndPos = new Vector3(0, -1.32f, 0);

    private void Start()
    {
        buttonMain = transform.GetChild(0);
        Vector3 buttonStartPos = buttonMain.transform.position;
    }



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject.tag == "Player")
        {
            buttonPressed();
        }
        else if (other.GetComponent<Rigidbody>() != null)
        {
            buttonPressed();
        }

    }

    private void OnTriggerExit(Collider other)
    {

            buttonReleased();

    }


    private void buttonPressed()
    {
         door.SetOpened(true);
        LeanTween.moveLocal(buttonMain.gameObject, buttonEndPos, 0.15f);
    }

    private void buttonReleased()
    {
        door.SetOpened(false);
        LeanTween.moveLocal(buttonMain.gameObject, buttonStartPos, 0.15f);
    }
}
