using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class ChasePlayer : MonoBehaviour
{

    //From Krissy#1337
    /*
    The MIT License (MIT)
    Copyright 2023 Krissy
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */

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
