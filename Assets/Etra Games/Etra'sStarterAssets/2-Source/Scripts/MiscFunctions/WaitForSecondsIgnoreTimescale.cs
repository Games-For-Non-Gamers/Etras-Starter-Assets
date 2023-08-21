using UnityEngine;

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
            return Time.realtimeSinceStartup < (startTime + duration);
        }
    }
}
