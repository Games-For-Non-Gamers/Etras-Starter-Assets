using UnityEngine;

namespace EtrasStarterAssets
{
    public class PlaySoundOnce : MonoBehaviour
    {
        public AudioClip sound;

        public void PlaySound()
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }
}
