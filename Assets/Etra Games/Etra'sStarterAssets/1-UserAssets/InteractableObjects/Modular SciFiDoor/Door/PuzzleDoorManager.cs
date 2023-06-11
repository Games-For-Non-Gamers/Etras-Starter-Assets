using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDoorManager : MonoBehaviour
{
    //References
    private PuzzleDoor puzzleDoor;

    //Variables
    public PuzzleTrigger[] doorTriggers;
    // Start is called before the first frame update
    void Start()
    {
        puzzleDoor = GetComponent<PuzzleDoor>();
    }

    // Update is called once per frame
    void Update()
    {
        bool check = CheckIfAllDoorTriggersAreTrue();
        bool state = puzzleDoor.GetOpenState();
        if(check && !state)
        {
            puzzleDoor.ToggleOpened();
        }
        else if(!check && state)
        {
            puzzleDoor.ToggleOpened();
        }
    }

    private bool CheckIfAllDoorTriggersAreTrue()
    {
        foreach (PuzzleTrigger item in doorTriggers)
        {
            if(!item.triggered)
            {
                return false;
            }
        }
        return true;
    }
}
