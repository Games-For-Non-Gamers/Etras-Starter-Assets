using UnityEngine;

namespace Etra.StarterAssets.Source
{
    public class ParticleSystemMultiPlayer : MonoBehaviour
    {
        public ParticleSystem[] particleSystems;

        public void Play()
        {
            foreach (var particleSystem in particleSystems)
            {
                particleSystem.Play();
            }
        }

        public void Stop()
        {
            foreach (var particleSystem in particleSystems)
            {
                particleSystem.Stop();
            }
        }
    }
}
