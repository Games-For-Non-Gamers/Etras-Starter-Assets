using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class OpenLink : MonoBehaviour
    {
        public string urlToOpen = "https://patreon.com/Games4Nongamers"; // Change this to the URL you want to open.

        public void OpenURL()
        {
            Application.OpenURL(urlToOpen);
        }
    }
}
