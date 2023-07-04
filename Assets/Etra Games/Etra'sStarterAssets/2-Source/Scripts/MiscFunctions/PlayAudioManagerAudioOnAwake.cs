using EtrasStarterAssets;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class PlayAudioManagerAudioOnAwake : MonoBehaviour
    {
        // Play first sound
        void Start()
        {
            GetComponent<AudioManager>().Play(GetComponent<AudioManager>().sounds[0]);
        }

    }
}