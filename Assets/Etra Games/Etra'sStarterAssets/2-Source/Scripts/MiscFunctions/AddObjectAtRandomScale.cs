using UnityEngine;

namespace Etra.StarterAssets
{
    public class AddObjectAtRandomScale : MonoBehaviour
    {
        public void scaleObjectWithRange(Vector2 internalRandomScaleRange)
        {

            float randomScale = UnityEngine.Random.Range(internalRandomScaleRange.x, internalRandomScaleRange.y);
            this.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            foreach (Transform child in transform)
            {
                child.localScale = new Vector3(randomScale, randomScale, randomScale);
            }
        }
    }
}