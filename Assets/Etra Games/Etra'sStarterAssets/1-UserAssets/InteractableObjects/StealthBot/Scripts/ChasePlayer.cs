using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class ChasePlayer : MonoBehaviour
{

    public Vector3 destination;
    public Transform Player, patrol;
    public NavMeshAgent agent;
    public GameObject cube;
    public bool spotted;
    public bool searchStarted;
    public float searchTime;
    public float viewDistance;
    public float viewAngle;

    void Update()
    {
        findThePlayer();
        if (searchStarted)
        {
            destination = Player.position;
        } else
        {
            destination = patrol.position;
        }
        agent.destination = destination;
    }

    IEnumerator search()
    {
        yield return new WaitForSeconds(searchTime);
        searchStarted = false;
        Debug.Log("eh i give up");
    }

    void findThePlayer()
    {
        if (Vector3.Distance(transform.position, Player.position) < viewDistance)
        {
            Debug.Log("Player in range");
            Vector3 directionToPlayer = (Player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.green);
            if (angleBetweenGuardAndPlayer < viewAngle / 2)
            {
                Debug.Log("Angle looks good");
                RaycastHit objectHit;
                if (Physics.Raycast(transform.position, directionToPlayer, out objectHit, viewDistance))
                {
                    if (objectHit.transform.tag == "Player")
                    {
                        searchStarted = true;
                        Debug.Log("player detected lmfao");
                        StartCoroutine(search());
                    }
                }
            }
        }
    }
}
