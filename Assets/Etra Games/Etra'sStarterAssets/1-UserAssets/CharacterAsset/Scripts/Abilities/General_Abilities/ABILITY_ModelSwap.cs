using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class ABILITY_ModelSwap : EtraAbilityBaseClass
{
    StarterAssetsInputs starterAssetsInputs;

    [SerializeField]
    MeshFilter meshFilter;

    [SerializeField]
    Mesh[] idleMeshes, walkMeshes, jumpMeshes, fallMeshes;

    [SerializeField]
    float updateMeshTimer, updateMeshTimerMax, updateTimerMaxWalk, updateTimerMaxSprint;

    [SerializeField]
    int meshIndex;

    bool jumping, falling;

    bool grounded;

    public override void abilityStart()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        mainController = GetComponentInParent<EtraCharacterMainController>();
    }

    public override void abilityUpdate()
    {
        if(!enabled)
        {
            return;
        }

        if(!mainController.Grounded)                        // if character is not grounded; use jumpMeshes or fallMeshes
        {
            grounded = false;

            if (updateMeshTimerMax == updateTimerMaxSprint)
            {
                updateMeshTimerMax = updateTimerMaxWalk;
            }

            if (!jumping && !falling)                       // first check if the player is in the air because they jumped or they are falling
            {
                if (starterAssetsInputs.jump)
                {
                    meshFilter.mesh = jumpMeshes[0];
                    jumping = true;
                    falling = false;
                }
                else
                {
                    meshFilter.mesh = fallMeshes[0];
                    falling = true;
                    jumping = false;
                }

                meshIndex = 0;                               // reset the index and timer before iterating through jump meshes or fall meshes
                updateMeshTimer = 0;
            }

            if (jumping)                                     // use jump meshes; after iterating through all jump meshes, switch to fall meshes until landing
            {
                if(updateMeshTimer > updateMeshTimerMax)
                {
                    meshIndex++;
                    updateMeshTimer = 0;
                }
                else
                {
                    updateMeshTimer += Time.deltaTime;
                }

                if(meshIndex >= jumpMeshes.Length)
                {
                    meshFilter.mesh = fallMeshes[0];
                    jumping = false;
                    falling = true;
                }
                else
                {
                    meshFilter.mesh = jumpMeshes[meshIndex];
                }
            }
            else if(falling)                                // use fall meshes
            {
                if (updateMeshTimer > updateMeshTimerMax)
                {
                    meshIndex++;
                    updateMeshTimer = 0;
                }
                else
                {
                    updateMeshTimer += Time.deltaTime;
                }

                if (meshIndex >= fallMeshes.Length)
                {
                    meshIndex = 0;
                }

                meshFilter.mesh = fallMeshes[meshIndex];
            }
        }
        else if(mainController.Grounded)                    // character is grounded; use idleMeshes or walkMeshes
        {
            if(!grounded)                                   // if character was in the ungrounded state, reset the animation to be a grounded animation
            {
                grounded = true;

                if(starterAssetsInputs.move.magnitude > 0)
                {
                    meshFilter.mesh = walkMeshes[0];
                    meshIndex = 0;
                }
                else
                {
                    meshFilter.mesh = idleMeshes[0];
                    meshIndex = 0;
                }
            }

            if(starterAssetsInputs.move.magnitude > 0)     // use walk meshes
            {
                if(starterAssetsInputs.sprint)
                {
                    updateMeshTimerMax = updateTimerMaxSprint;
                }
                else
                {
                    updateMeshTimerMax = updateTimerMaxWalk;
                }

                if(updateMeshTimer > updateMeshTimerMax)
                {
                    meshIndex++;
                    updateMeshTimer = 0;
                }
                else
                {
                    updateMeshTimer += Time.deltaTime;
                }

                if(meshIndex >= walkMeshes.Length)
                {
                    meshIndex = 0;
                }

                meshFilter.mesh = walkMeshes[meshIndex];
            }
            else                                           // use idle meshes
            {
                if(updateMeshTimerMax == updateTimerMaxSprint)
                {
                    updateMeshTimerMax = updateTimerMaxWalk;
                }

                if (updateMeshTimer > updateMeshTimerMax)
                {
                    meshIndex++;
                    updateMeshTimer = 0;
                }
                else
                {
                    updateMeshTimer += Time.deltaTime;
                }

                if (meshIndex >= idleMeshes.Length)
                {
                    meshIndex = 0;
                }

                meshFilter.mesh = idleMeshes[meshIndex];
            }
        }
    }
}
