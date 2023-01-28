using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactionizer : MonoBehaviour
{
    public GameObject cursor; // The cursor that is shown when looking at an object you can interact with
    public string interactableTag = "Interactable"; // The tag of the objects you can interact with
    public float interactDistance = 2.0f; // The distance you can interact with an object
    private ObjectInteraction previousObject; // Previous object gotten in the raycast

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the player is looking at an object with the interactable tag (Camera.main raycast hits an object with the interactable tag)
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance) && hit.transform.tag == interactableTag)
        {
            ObjectInteraction interaction = hit.transform.GetComponent<ObjectInteraction>(); // Get the object's interaction script

            if (interaction.isInteractable)
            {
                if (previousObject != null)
                {
                    if (previousObject != interaction)
                    {
                        interaction.onHover.Invoke(); // Call the previous object's onHover event
                        previousObject.onEndHover.Invoke(); // Call the previous object's onEndHover event
                    }
                }
                else
                {
                    interaction.onHover.Invoke(); // Call the object's onHover event
                }

                previousObject = interaction; // Set the previous object to the current object

                if (cursor != null)
                {
                    // If the object is currently interactable
                    cursor.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E)) // TODO: Change this to a custom input, hopefully with InputSystem. However, this would require huge changes to the existing code, I don't think that's a good idea without a Git repository.
                {
                    // Interact with the object
                    hit.transform.gameObject.GetComponent<ObjectInteraction>().Interact();
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    // End the interaction with the object
                    hit.transform.gameObject.GetComponent<ObjectInteraction>().onEndInteract.Invoke();
                    hit.transform.gameObject.GetComponent<ObjectInteraction>().onPress.Invoke();
                }
            }
            else
            {
                if (cursor != null)
                {
                    // Hide the cursor
                    cursor.SetActive(false);
                }

                if (previousObject != null)
                {
                    previousObject.onEndHover.Invoke(); // Call the previous object's onEndHover event
                    previousObject.onEndInteract.Invoke(); // Call the previous object's onEndInteract event
                }

                previousObject = null; // Set the previous object to null
            }
        }
        else
        {
            if (cursor != null)
            {
                // Hide the cursor
                cursor.SetActive(false);
            }

            if (previousObject != null)
            {
                previousObject.onEndHover.Invoke(); // Call the previous object's onEndHover event
                previousObject.onEndInteract.Invoke(); // Call the previous object's onEndInteract event
            }

            previousObject = null; // Set the previous object to null
        }
    }
}
