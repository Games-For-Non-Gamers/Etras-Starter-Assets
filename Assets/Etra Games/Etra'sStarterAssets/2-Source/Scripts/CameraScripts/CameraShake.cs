using System.Collections;
using UnityEngine;

namespace Etra.StarterAssets.Source.Camera
{
    public class CameraShake : MonoBehaviour
    {
        public IEnumerator Shake(float camMoveRange, float duration, float magnitude)
        {
            var originalPos = transform.localPosition;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-camMoveRange, camMoveRange) * magnitude;
                float y = Random.Range(-camMoveRange, camMoveRange) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;

                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
}
