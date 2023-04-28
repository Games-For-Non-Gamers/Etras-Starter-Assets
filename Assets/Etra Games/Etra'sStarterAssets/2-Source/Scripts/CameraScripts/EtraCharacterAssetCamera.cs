using Cinemachine;
using UnityEngine;

namespace Etra.StarterAssets.Source.Camera
{
    public class EtraCharacterAssetCamera : MonoBehaviour
    {
        CinemachineVirtualCamera cam;

        void Reset()
        {
            cam = GetComponent<CinemachineVirtualCamera>();
            cam.Follow = GameObject.Find("EtraPlayerCameraRoot").transform;
        }
    }
}
