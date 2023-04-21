using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INTERACTABLE_Target : MonoBehaviour
{
    public int trigger;
    public LineRenderer lr;
    public Transform center;
    public Transform lrPoint;

    public Transform sparker;


    public Transform[] ropePoints;

    
    // Start is called before the first frame update
    void OnValidate ()
    {
        Vector3 [] positions = new Vector3[ropePoints.Length+2];

        positions[0] = center.position;
        positions[1] = lrPoint.position;

        for (int i = 2; i < ropePoints.Length+2; i++)
        {
            positions[i] = ropePoints[i-2].position;
        }

        lr = GetComponent<LineRenderer>();
        lr.positionCount = ropePoints.Length + 2;
        lr.SetPositions(positions);

        sparker.position = positions[ropePoints.Length+1];
    }

    // Update is called once per frame
    bool coroutineStarted = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!coroutineStarted)
            {
                coroutineStarted = true;
                StartCoroutine(burnRope());
            }
        }
    }


    IEnumerator burnRope()
    {

        //Use LeanTween.value (Vector3) to make the rope shrink
        //( gameObject:GameObject  callOnUpdate:Action<Vector3>  from:Vector3  to:Vector3  time:float 
        //Example from ETG new:  LeanTween.value(self, inital, final, 1f).setOnUpdate((float speedValue) => { speed = speedValue; });

        //Math to make "timeToShrink" work
        /*
        float distanceBetweenAllNodes = 0;
        for (int i = 0; i < spline.nodes.Count - 1; i++)
        {
            distanceBetweenAllNodes += Vector3.Distance(spline.nodes[i].Position, spline.nodes[i + 1].Position);
        }

        float sparkUnitsPerSecond = distanceBetweenAllNodes / timeToShrink;

        for (int i = 0; i < spline.nodes.Count-1; i++)
        {
            float distanceBetweenNodes = Vector3.Distance(spline.nodes[i].Position, spline.nodes[i + 1].Position);
            float timeToNext = distanceBetweenNodes / sparkUnitsPerSecond;
            LeanTween.moveLocal(sparker, spline.nodes[i + 1].Position, timeToNext);
            yield return new WaitForSeconds(timeToNext);

        }
 
        */

        yield return new WaitForSeconds(1);
    }
}
