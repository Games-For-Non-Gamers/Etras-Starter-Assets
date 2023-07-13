using UnityEngine;
using Cinemachine;

namespace Etra.StarterAssets.Source.Camera
{
    public class CinemachineShake : MonoBehaviour
    {
        public static CinemachineShake Instance { get; private set; }

        private CinemachineVirtualCamera cinemachineVirtualCamera;
        public bool shakeEnabled = true;
        public float shakeTimer;
        private float shakeTimerTotal;
        private float startingIntensity;
        private void Awake()
        {
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        }

        public void Start()
        {
            ResetCamera();
        }

        public void ResetCamera()
        {
            var cinemachineBasicMultiChannelPerlin =
    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;

            startingIntensity = 0;
            shakeTimerTotal = 0;
            shakeTimer = 0;
        }

        public void ShakeCamera(float intensity, float time)
        {
            if (!shakeEnabled)
            {
                ResetCamera();
                return;
            }

            var cinemachineBasicMultiChannelPerlin =
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

            startingIntensity = intensity;
            shakeTimerTotal = time;
            shakeTimer = time;
        }

        public void ShakeCamera(Vector2 passedVars)
        {
            if (!shakeEnabled)
            {
                ResetCamera();
                return;
            }
            float intensity = passedVars.x;
            float time = passedVars.y;

            var cinemachineBasicMultiChannelPerlin =
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

            startingIntensity = intensity;
            shakeTimerTotal = time;
            shakeTimer = time;
        }


        private void Update()
        {

            //Make sure the live camera has the shake
            if (CinemachineCore.Instance.IsLive(cinemachineVirtualCamera))
            {
                if (Instance != this)
                {
                    Instance = this;
                }

            }

            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0)
                {
                    var cinemachineBasicMultiChannelPerlin =
                    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
                    Mathf.Lerp(startingIntensity, 0, 1 - shakeTimer / shakeTimerTotal);
                }
            }
        }
    }
}