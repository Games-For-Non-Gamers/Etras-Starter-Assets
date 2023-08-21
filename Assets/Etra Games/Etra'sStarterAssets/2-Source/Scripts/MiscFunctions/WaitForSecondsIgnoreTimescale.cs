using UnityEngine;

namespace Etra.StarterAssets
{
    public class WaitForSecondsIgnoreTimeScale : CustomYieldInstruction
    {
        private float startTime;
        private float duration;

        public WaitForSecondsIgnoreTimeScale(float seconds)
        {
            startTime = Time.realtimeSinceStartup;
            duration = seconds;
        }

        public override bool keepWaiting
        {
            get
            {
                // Calculate the current time without considering time scale
                float currentTime = Time.realtimeSinceStartup;

                // Check if the time duration has passed
                return currentTime < (startTime + duration);
            }
        }
    }
}