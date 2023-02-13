using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SciFiDoor : MonoBehaviour
{
    public Animator animator;

    public void SetOpened(bool open)
    {
        animator.SetBool("Opened", open);
    }

    public void ToggleOpened()
    {
        animator.SetBool("Opened", !animator.GetBool("Opened"));
    }
}
