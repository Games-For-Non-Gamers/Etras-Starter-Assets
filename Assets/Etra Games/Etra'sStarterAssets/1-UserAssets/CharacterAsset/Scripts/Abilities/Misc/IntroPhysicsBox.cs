using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Etra.StarterAssets
{
    public class IntroPhysicsBox : MonoBehaviour
    {
        // If picked up, then turn on second hitbox
        public GameObject secondEventHitbox;
        bool eventDone = false;
        void Start()
        {
            secondEventHitbox.SetActive(false);
        }

        // Update is called once per frame
        public void runIntroEvent()
        {
            if (!eventDone)
            {
                eventDone = true;
                secondEventHitbox.SetActive(true);
            }

            
        }
    }
}
