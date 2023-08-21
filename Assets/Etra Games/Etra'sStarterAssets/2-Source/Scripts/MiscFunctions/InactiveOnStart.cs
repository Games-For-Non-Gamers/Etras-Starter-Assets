using UnityEngine;

namespace Etra.StarterAssets
{
    public class InactiveOnStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.SetActive(false);
        }
    }
}
