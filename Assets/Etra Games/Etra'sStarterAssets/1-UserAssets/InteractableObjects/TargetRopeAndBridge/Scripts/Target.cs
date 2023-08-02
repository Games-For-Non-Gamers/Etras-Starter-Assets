using Etra.StarterAssets.Source;
using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class Target : MonoBehaviour
    {
        [Header("Basics")]
        public float timeToShrinkRope = 5;
        public Bridge bridge;
        public Transform[] ropePoints;

        [Header("Moving Target")]
        public float moveToLocationsTime = 5;
        public Transform[] moveLocations;

        //References by previous dragging
        [HideInInspector] public Transform center;
        [HideInInspector] public Transform lrPoint;
        [HideInInspector] public Transform animScaler;
        [HideInInspector] public Transform sparker;
        [HideInInspector] public LineRenderer ropeLine;
        //References by code
        LineRenderer anchorLine;
        GameObject lineFuser;
        EtrasStarterAssets.AudioManager audioManager;
        //Activated var
        [HideInInspector] public bool activated = false;
        ParticleSystem impact;
        // This function is called when a change is made in the Inspector
        public void OnValidate()
        {
            updateRopes();
        }

        private void Start()
        {
            // Calls the updateRopes() function to update the rope
            updateRopes();
            // Sets the sparker object as active and scales it to zero
            sparker.gameObject.SetActive(true);
            LeanTween.scale(sparker.gameObject, Vector3.zero, 0);
            sparker.gameObject.SetActive(false);
            // Gets the LineRenderer and GameObject components from the child objects of the target object
            anchorLine = transform.GetChild(0).GetChild(1).gameObject.GetComponent<LineRenderer>();
            lineFuser = transform.GetChild(0).GetChild(3).gameObject;

            // If there are two or more moveLocations
            if (moveLocations.Length >= 2)
            {
                // Set lineFuser and anchorLine objects as active and enabled respectively
                lineFuser.SetActive(true);
                anchorLine.enabled = true;
                anchorLine = transform.GetChild(0).GetChild(1).gameObject.GetComponent<LineRenderer>();
                // Starts the coroutine moveToMovePositions()
                StartCoroutine(moveToMovePositions());
            }
            else
            {
                // Set lineFuser and anchorLine objects as inactive and disabled respectively
                lineFuser.SetActive(false);
                anchorLine.enabled = false;
            }

            audioManager = GetComponent<EtrasStarterAssets.AudioManager>();
            impact = this.transform.Find("Source Objects (DON'T REARRANGE)").Find("Impact").GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            // If the target is a moving target, constantly update the anchorLine
            if (moveLocations.Length >= 2)
            {
                anchorLine.SetPosition(1, animScaler.position);
            }
        }

        bool ongoing = true; // helper for stopping while loop
        IEnumerator moveToMovePositions()
        {
            //Set anchor line
            var anchorLine = transform.GetChild(0).GetChild(1).gameObject.GetComponent<LineRenderer>();
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
                distanceBetweenAllNodes += Vector3.Distance(moveLocations[i].position, moveLocations[i + 1].position);
            }
            float sparkUnitsPerSecond = distanceBetweenAllNodes / moveToLocationsTime;

            //Loop going to positions
            int incrementer = 0;
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

        // The function calculates the new position of the ropes based on the current positions of the rope points and the center and lrPoint positions
        //For some reason he line renderer will only update properly if points are deleted from the end of the positions array
        //So that's a bit wierd, but I worked with it
        public void updateRopes()
        {


            //Set the positions array size
            Vector3[] positions = new Vector3[ropePoints.Length + 2];

            //Always set first two points to center and lr point
            positions[positions.Length - 1] = center.position;
            positions[positions.Length - 2] = lrPoint.position;

            //add additional points to positions
            int inc = 0;
            for (int i = positions.Length - 3; i >= 0; i--)
            {
                positions[i] = ropePoints[inc].position;
                inc++;
            }

            //Get ropeline and set the actual positions of the line renderer
            ropeLine = transform.GetChild(0).GetChild(0).gameObject.GetComponent<LineRenderer>();
            ropeLine.positionCount = ropePoints.Length + 2;
            ropeLine.SetPositions(positions);

            //Set the sparker location to the last index (or target center)
            sparker.position = positions[ropeLine.positionCount - 1];
        }

        //Here is the full hit to rope burning animation
        bool coroutineStarted = false;
        public void targetHit()
        {
            if (!coroutineStarted)
            {
                coroutineStarted = true;
                StartCoroutine(burnRope());
            }

        }
        IEnumerator burnRope()
        {

            ongoing = false;
            StopCoroutine(moveToMovePositions());
            LeanTween.cancel(this.gameObject);

            ropeLine = transform.GetChild(0).GetChild(0).gameObject.GetComponent<LineRenderer>();
            //from end to beggining. So odd
            audioManager.Play("LightSparkle");
            LeanTween.scale(animScaler.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.25f).setEaseInOutSine();
            yield return new WaitForSeconds(0.25f);
            LeanTween.moveLocal(animScaler.gameObject, Vector3.zero, 0.5f).setEaseInOutSine();
            LeanTween.scale(animScaler.gameObject, Vector3.zero, 0.5f).setEaseInOutSine();
            yield return new WaitForSeconds(0.45f);
            LeanTween.scale(lineFuser, Vector3.zero, 0.05f);
            lineFuser.SetActive(false);
            sparker.gameObject.SetActive(true);
            LeanTween.scale(sparker.gameObject, new Vector3(0.2f, 0.2f, 0.11f), 0.05f).setEaseInOutSine();
            impact.Play();
            yield return new WaitForSeconds(0.05f);
            audioManager.Play("TargetPop");
            audioManager.Play("BurningSparkler");
            anchorLine.enabled = false;
            animScaler.gameObject.SetActive(false);


            float distanceBetweenAllNodes = 0;
            for (int i = 0; i < ropeLine.positionCount - 1; i++)
            {
                distanceBetweenAllNodes += Vector3.Distance(ropeLine.GetPosition(i), ropeLine.GetPosition(i + 1));
            }


            float sparkUnitsPerSecond = distanceBetweenAllNodes / timeToShrinkRope;

            for (int i = ropeLine.positionCount - 1; i > 0; i--)
            {
                float distanceBetweenNodes = Vector3.Distance(ropeLine.GetPosition(i), ropeLine.GetPosition(i - 1));
                float timeToNext = distanceBetweenNodes / sparkUnitsPerSecond;
                LeanTween.move(sparker.gameObject, ropeLine.GetPosition(i - 1), timeToNext);
                float sparkRotateSpeedDivider = timeToShrinkRope / 5;

                LeanTween.value(gameObject, ropeLine.GetPosition(i), ropeLine.GetPosition(i - 1), timeToNext).setOnUpdate((Vector3 newPosValue) =>
                {

                    ropeLine.SetPosition(i, newPosValue);
                    var targetDirection = ropeLine.GetPosition(i - 1) - sparker.position;
                    float singleStep = 20 / sparkRotateSpeedDivider * Time.deltaTime;
                    Vector3 newDirection = Vector3.RotateTowards(sparker.transform.forward, targetDirection, singleStep, 0.9f*Time.deltaTime);
                    Quaternion lookRot = Quaternion.LookRotation(newDirection);
                    sparker.transform.rotation = lookRot;

                });
                yield return new WaitForSeconds(timeToNext);
                ropeLine.positionCount--;
            }


         //   LeanTween.scale(sparker.gameObject, Vector3.zero, 0.2f).setEaseInOutSine();
            ParticleSystem[] particles = sparker.GetComponentsInAllChildren<ParticleSystem>();

            foreach (ParticleSystem particle in particles)
            {
                particle.Stop();
            }

            yield return new WaitForSeconds(0.05f);
            audioManager.Stop("BurningSparkler");
            audioManager.Play("MedSparkle");
            ropeLine.enabled = false;
            activated = true;
            bridge.checkActivate();
            yield return new WaitForSeconds(1);
            sparker.gameObject.SetActive(false);
        }
    }
}
