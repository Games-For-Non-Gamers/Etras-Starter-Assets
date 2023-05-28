using UnityEngine;

namespace EtrasStarterAssets.Interactables
{
    public class TreadmillSoundPlayer : MonoBehaviour
    {

        EtrasStarterAssets.AudioManager audioManager;
        // Start is called before the first frame update
        void Start()
        {
            audioManager = GetComponent<EtrasStarterAssets.AudioManager>();
            audioManager.Play("Treadmill");
        }

    }

}

