using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class INTERACTABLE_Target : MonoBehaviour, ISerializationCallbackReceiver
{
    [Header("Basics")]
    public float timeToShrink = 5;
    public Transform[] ropePoints;

    //
    [HideInInspector]public Transform center;
    [HideInInspector] public Transform lrPoint;
    [HideInInspector] public Transform animScaler;
    public Transform sparker;
    private LineRenderer lr;

    

    // Start is called before the first frame update
    public void OnBeforeSerialize()
    {
        
        Vector3[] burningPositions = new Vector3[ropePoints.Length + 2];
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

        sparker.position = positions[0];
        
    }

    public void OnAfterDeserialize()
    {

    }


    private void Start()
    {
        OnBeforeSerialize();
    }

    // Update is called once per frame
    bool coroutineStarted = false;
    public void targetHit()
    {
        if (!coroutineStarted)
        {
            coroutineStarted = true;
            StartCoroutine(burnRope());
        }

    }

    //also do the scale animation in Leantween?


    IEnumerator burnRope()
    {
        LeanTween.scale(animScaler.gameObject, new Vector3(1.2f,1.2f,1.2f), 0.25f).setEaseInOutSine();
        yield return new WaitForSeconds(0.25f);
        LeanTween.scale(animScaler.gameObject, Vector3.zero, 0.5f).setEaseInOutSine();
        yield return new WaitForSeconds(0.5f);


        float distanceBetweenAllNodes = 0;
        for (int i = 0; i < lr.positionCount - 1; i++)
        {
            distanceBetweenAllNodes += Vector3.Distance(lr.GetPosition(i), lr.GetPosition(i+1));
        }


        float sparkUnitsPerSecond = distanceBetweenAllNodes / timeToShrink;

        int posCount = lr.positionCount - 1;
        for (int i = 0; i < posCount; i++)
        {
            float distanceBetweenNodes = Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1));
            float timeToNext = distanceBetweenNodes / sparkUnitsPerSecond;
            LeanTween.moveLocal(sparker.gameObject, lr.GetPosition(1), timeToNext);
            LeanTween.value(this.gameObject, lr.GetPosition(0), lr.GetPosition(1), timeToNext).setOnUpdate((Vector3 newPosValue) => { lr.SetPosition(0, newPosValue); });
            yield return new WaitForSeconds(timeToNext);

            for (int j = 0; j < lr.positionCount-1; j++)
            {
                lr.SetPosition(j, lr.GetPosition(j+1));
            }
            lr.positionCount--;
        }

        lr.enabled = false;

        //Math to make "timeToShrink" work
        
    }


}
