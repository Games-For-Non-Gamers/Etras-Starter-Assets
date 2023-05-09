using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class SciFiDoor : MonoBehaviour
    {
        //From @aMySour
        /*
        The MIT License (MIT)
        Copyright 2023 amysour
        Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        */

        //References
        public Animator animator;
        EtrasStarterAssets.AudioManager audioManager;
        // Start is called before the first frame update
        void Start()
        {
            audioManager = GetComponent<EtrasStarterAssets.AudioManager>();
        }

        //Functions for changeing or toggling the door state
        public void SetOpened(bool open)
        {
            if (open)
            {
                audioManager.Play("ScifiDoorOpen");
                audioManager.Stop("ScifiDoorClose");
            }
            else
            {
                audioManager.Play("ScifiDoorClose");
                audioManager.Stop("ScifiDoorOpen");
            }

            animator.SetBool("Opened", open);
        }

        public void ToggleOpened()
        {
            SetOpened(!animator.GetBool("Opened"));
        }
    }
}
