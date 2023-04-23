using EtrasStarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class INTERACTABLE_Target : MonoBehaviour  /* , ISerializationCallbackReceiver*/
{
    [Header("Basics")]
    public float timeToShrinkRope = 5;
    public INTERACTABLE_Bridge bridge;
    public Transform[] ropePoints;

    [Header("Moving Target")]
    public float moveToLocationsTime = 5;
    public Transform[] moveLocations;


    [HideInInspector]public Transform center;
    [HideInInspector] public Transform lrPoint;
    [HideInInspector] public Transform animScaler;
    [HideInInspector] public bool activated = false;
    [HideInInspector] public Transform sparker;
    [HideInInspector] public LineRenderer ropeLine;
    LineRenderer anchorLine;
    GameObject lineFuser;


    // Start is called before the first frame update
    public void OnValidate()
    {
        updateRopes();
    }


    private void Start()
    {
        updateRopes();
        sparker.gameObject.SetActive(true);
        LeanTween.scale(sparker.gameObject, Vector3.zero, 0);
        sparker.gameObject.SetActive(false);
        anchorLine = transform.GetChild(0).GetChild(1).gameObject.GetComponent<LineRenderer>();
        lineFuser = transform.GetChild(0).GetChild(3).gameObject;

        if (moveLocations.Length >=2)
        {
            lineFuser.SetActive(true);
            anchorLine.enabled = true;
            anchorLine = transform.GetChild(0).GetChild(1).gameObject.GetComponent<LineRenderer>();
            StartCoroutine(moveToMovePositions());
        }
        else
        {
            lineFuser.SetActive(false);
            anchorLine.enabled = false;
        }
      
    }

    private void Update()
    {
        if (moveLocations.Length >= 2)
        {
            anchorLine.SetPosition(1, animScaler.position);
        }
    }
    bool ongoing = true;
    IEnumerator moveToMovePositions()
    {
        //Set anchor line
        LineRenderer anchorLine = transform.GetChild(0).GetChild(1).gameObject.GetComponent<LineRenderer>();
        anchorLine.positionCount = 2;
        Vector3[] anchorPositions = new Vector3[2];
        anchorLine.SetPositions(anchorPositions);
        anchorLine.SetPosition(0, animScaler.position);
        anchorLine.SetPosition(1, transform.GetChild(0).GetChild(2).GetChild(0).position);

        //Go to pos 0
        LeanTween.move(animScaler.gameObject, moveLocations[0].position, 0);


        //Get data for speed
        float distanceBetweenAllNodes = 0;
        for (int i = 0; i < moveLocations.Length - 1; i++)
        {
            distanceBetweenAllNodes += Vector3.Distance(moveLocations[i].position, moveLocations[i+1].position);
        }
        float sparkUnitsPerSecond = distanceBetweenAllNodes / moveToLocationsTime;

        //Loop going to positions
        int incrementer =0;
        while (ongoing)
        {
            int oldPosition = incrementer;
            incrementer++;

            if (incrementer >= moveLocations.Length)
            {
                incrementer = 0;
            }

            float distanceBetweenNodes = Vector3.Distance(moveLocations[oldPosition].position, moveLocations[incrementer].position);
            float timeToNext = distanceBetweenNodes / sparkUnitsPerSecond;
            LeanTween.move(animScaler.gameObject, moveLocations[incrementer].position, timeToNext);
            yield return new WaitForSeconds(timeToNext);
        }
    }

    public void updateRopes()
    {
        if (Application.isPlaying)
        {
            return;
        }
        Vector3[] positions = new Vector3[ropePoints.Length + 2];

        positions[positions.Length - 1] = center.position;
        positions[positions.Length - 2] = lrPoint.position;


        int inc = 0;
        for (int i = positions.Length - 3; i >= 0; i--)
        {
            positions[i] = ropePoints[inc].position;
            inc++;
        }

        ropeLine = transform.GetChild(0).GetChild(0).gameObject.GetComponent<LineRenderer>();
        ropeLine.positionCount = ropePoints.Length + 2;
        ropeLine.SetPositions(positions);

        sparker.position = positions[ropeLine.positionCount - 1];
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
        
        ongoing = false;
        StopCoroutine(moveToMovePositions());
        LeanTween.cancel(this.gameObject);

        ropeLine = transform.GetChild(0).GetChild(0).gameObject.GetComponent<LineRenderer>();
        //from end to beggining. So odd

        LeanTween.scale(animScaler.gameObject, new Vector3(1.2f,1.2f,1.2f), 0.25f).setEaseInOutSine();
        yield return new WaitForSeconds(0.25f);
        LeanTween.moveLocal(animScaler.gameObject, Vector3.zero, 0.5f).setEaseInOutSine();
        LeanTween.scale(animScaler.gameObject, Vector3.zero, 0.5f).setEaseInOutSine();
        yield return new WaitForSeconds(0.45f);
        LeanTween.scale(lineFuser, Vector3.zero, 0.05f);
        lineFuser.SetActive(false);
        sparker.gameObject.SetActive(true); 
        LeanTween.scale(sparker.gameObject, new Vector3(0.2f, 0.2f, 0.11f), 0.05f).setEaseInOutSine();
        yield return new WaitForSeconds(0.05f);
        anchorLine.enabled = false;
        animScaler.gameObject.SetActive(false);


        float distanceBetweenAllNodes = 0;
        for (int i = 0; i < ropeLine.positionCount - 1; i++)
        {
            distanceBetweenAllNodes += Vector3.Distance(ropeLine.GetPosition(i), ropeLine.GetPosition(i+1));
        }


        float sparkUnitsPerSecond = distanceBetweenAllNodes / timeToShrinkRope;
        //delete from end to beggining. hmm

        for (int i = ropeLine.positionCount - 1; i > 0; i--)
        {
            float distanceBetweenNodes = Vector3.Distance(ropeLine.GetPosition(i), ropeLine.GetPosition(i-1));
            float timeToNext = distanceBetweenNodes / sparkUnitsPerSecond;
            LeanTween.move(sparker.gameObject, ropeLine.GetPosition(i-1), timeToNext);
            float sparkRotateSpeedDivider = timeToShrinkRope/5; 

            LeanTween.value(this.gameObject, ropeLine.GetPosition(i), ropeLine.GetPosition(i - 1), timeToNext).setOnUpdate((Vector3 newPosValue) => {

                ropeLine.SetPosition(i, newPosValue);
                Vector3 targetDirection = ropeLine.GetPosition(i-1) - sparker.position;
                float singleStep = (20/ sparkRotateSpeedDivider) * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(sparker.transform.forward, targetDirection, singleStep, 0.0f);
                Quaternion lookRot = Quaternion.LookRotation(newDirection);
                sparker.transform.rotation = lookRot;

            });
            yield return new WaitForSeconds(timeToNext);
            ropeLine.positionCount--;
        }

        LeanTween.scale(sparker.gameObject, Vector3.zero, 0.05f).setEaseInOutSine();
        yield return new WaitForSeconds(0.05f);
        ropeLine.enabled = false;
        activated=true;
        sparker.gameObject.SetActive(false);
        bridge.checkActivate();
    }


}
