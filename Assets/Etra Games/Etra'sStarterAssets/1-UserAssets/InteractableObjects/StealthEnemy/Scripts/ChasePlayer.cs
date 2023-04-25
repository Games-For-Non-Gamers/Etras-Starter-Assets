using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

namespace EtrasStarterAssets
{
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
        public Material NoneFound;
        public Material PlayerFound;
        GameObject enemyEye;

        private void Start()
        {
            enemyEye = transform.Find("TiltRoot").Find("Base").Find("BaseTop").Find("Center").Find("Body").Find("NeckRoatator").Find("Neck").Find("Head").Find("Eye").gameObject;
        }

        void Update()
        {
            findThePlayer();
            if (searchStarted)
            {
                // Set eye material to player found material, and set flashlight to material color
                enemyEye.GetComponent<MeshRenderer>().material = PlayerFound;
                enemyEye.transform.Find("Spot Light").GetComponent<Light>().color = Color.red;
                destination = Player.position;
            }
            else
            {
                // Set eye material to none found material, and set flashlight to material color
                enemyEye.GetComponent<MeshRenderer>().material = NoneFound;
                enemyEye.transform.Find("Spot Light").GetComponent<Light>().color = Color.green;
                destination = patrol.position;
            }
            agent.destination = destination;
            if (agent.remainingDistance < 1)
            {
                // "Kill" player, respawn at last checkpoint.
                if (EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>())
                {
                    EtraCharacterMainController.Instance.etraAbilityManager.GetComponent<ABILITY_CheckpointRespawn>().teleportToCheckpoint();
                }
                else
                {
                    Debug.LogWarning("To Use the Checkpoint Teleporter ABILITY_CheckpointRespawn must be added to your character's ability mannager. ");
                }
                searchStarted = false;
            }
        }

        IEnumerator search()
        {
            yield return new WaitForSeconds(searchTime);
            searchStarted = false;
        }

        void findThePlayer()
        {
            if (Vector3.Distance(transform.position, Player.position) < viewDistance)
            {
                Vector3 directionToPlayer = (Player.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.green);
                if (angleBetweenGuardAndPlayer < viewAngle / 2)
                {
                    RaycastHit objectHit;
                    if (Physics.Raycast(transform.position, directionToPlayer, out objectHit, viewDistance))
                    {
                        if (objectHit.transform.tag == "Player")
                        {
                            searchStarted = true;
                            StartCoroutine(search());
                        }
                    }
                }
            }
        }

        public bool ifPlayerFound()
        {
            if (Vector3.Distance(transform.position, Player.position) < viewDistance)
            {
                Vector3 directionToPlayer = (Player.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.green);
                if (angleBetweenGuardAndPlayer < viewAngle / 2)
                {
                    RaycastHit objectHit;
                    if (Physics.Raycast(transform.position, directionToPlayer, out objectHit, viewDistance))
                    {
                        if (objectHit.transform.tag == "Player")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}