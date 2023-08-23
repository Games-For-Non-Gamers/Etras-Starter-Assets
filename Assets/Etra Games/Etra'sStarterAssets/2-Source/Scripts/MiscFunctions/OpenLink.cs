using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class OpenLink : MonoBehaviour
    {
        public string urlToOpen = "https://www.patreon.com/Games4NonGamers/shop"; // Change this to the URL you want to open.

        public void OpenURL()
        {
            Application.OpenURL(urlToOpen);
        }
    }
}
