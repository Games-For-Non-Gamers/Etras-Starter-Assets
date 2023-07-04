using UnityEngine;

namespace EtrasStarterAssets
{
    public class PhysicsBoxAudio : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > 2)
            {
                GetComponent<AudioManager>().Play("BoxCrash");
            }
           
        }
    }

}
