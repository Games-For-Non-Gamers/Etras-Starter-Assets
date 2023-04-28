using UnityEngine;

namespace Etra.StarterAssets.Source.Interactions
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
