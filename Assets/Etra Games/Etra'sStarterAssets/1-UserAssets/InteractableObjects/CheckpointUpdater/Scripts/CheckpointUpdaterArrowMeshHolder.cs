using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets.Interactables
{
    public class CheckpointUpdaterArrowMeshHolder : MonoBehaviour
    {

        public MeshRenderer arrowBase;
        public MeshRenderer arrowRight;
        public MeshRenderer arrowLeft;

        public void disableMeshes()
        {
            arrowBase.enabled = false;
            arrowRight.enabled = false;
            arrowLeft.enabled = false;
        }

        public void enableMeshes()
        {
            arrowBase.enabled = true;
            arrowRight.enabled = true;
            arrowLeft.enabled = true;
        }

    }
}
