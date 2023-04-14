using UnityEngine;

namespace EtrasStarterAssets{
    public class ParticleSystemMultiPlayer : MonoBehaviour
    {
        public ParticleSystem[] particleSystems;

        public void Play()
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Play();
            }
        }

        public void Stop()
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Stop();
            }
        }
    }
}
