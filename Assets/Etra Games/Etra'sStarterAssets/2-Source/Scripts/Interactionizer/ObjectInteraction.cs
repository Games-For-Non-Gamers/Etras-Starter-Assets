using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ObjectInteraction : MonoBehaviour
{
    public bool isInteractable = true; // Whether the object is interactable
    public UnityEvent onPress; // The event that is called when a full successful interaction is made
    public UnityEvent onInteract; // The event that is called when the object is interacted with
    public UnityEvent onEndInteract; // The event that is called when the interaction is released
    public UnityEvent onHover; // The event that is called when the object is hovered over
    public UnityEvent onEndHover; // The event that is called when the object is no longer hovered over

    public void Interact()
    {
        // Call the event
        onInteract.Invoke();
    }

    public void SetInteractable(bool interactable)
    {
        // Set the interactable state
        isInteractable = interactable;
    }
}