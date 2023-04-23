using System.Collections;
using UnityEngine;

public class INTERACTABLE_Bridge : MonoBehaviour
{
    public INTERACTABLE_Target[] targets;
    public float timeToLower =1;
    private bool bridgeLowered = false;

    private void Start()
    {
        checkActivate();
    }

    public void checkActivate()
    {
        if (bridgeLowered)
        {
            return;
        }

        bool allActivated = true;
        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].activated)
            {
                allActivated = false;
            }
        }

        if (allActivated)
        {
            StartCoroutine(lowerBridge());
        }

    }

    IEnumerator lowerBridge()
    {
        bridgeLowered = true;
        yield return new WaitForSeconds(1);
        LeanTween.rotate(transform.GetChild(0).GetChild(0).gameObject, Vector3.zero, timeToLower).setEaseInOutSine();
    }

}
