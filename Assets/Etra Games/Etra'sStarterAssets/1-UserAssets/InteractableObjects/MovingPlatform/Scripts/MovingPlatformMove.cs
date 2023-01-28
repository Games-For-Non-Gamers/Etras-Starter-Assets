using System.Collections;
using UnityEngine;

public class MovingPlatformMove : MonoBehaviour
{
    [Header("Basics")]
    public float howLongToMove = 5.0f;
    public float howLongToWaitAtEndPos = 1.0f; 
    
    
    [Header ("Object References")]
    public GameObject platform;
    public Transform startPos;
    public Transform endPos;

    // Start is called before the first frame update
    void Start()
    {
        platform.transform.position = startPos.position;
        StartCoroutine(moveToEnd());
    }

    IEnumerator moveToStart()
    {
        LeanTween.move(platform, startPos, howLongToMove);
        yield return new WaitForSeconds(howLongToMove);
        yield return new WaitForSeconds(howLongToWaitAtEndPos);
        StartCoroutine(moveToEnd());
    }

    IEnumerator moveToEnd()
    {
        LeanTween.move(platform, endPos, howLongToMove);
        yield return new WaitForSeconds(howLongToMove);
        yield return new WaitForSeconds(howLongToWaitAtEndPos);
        StartCoroutine(moveToStart());
    }
}
