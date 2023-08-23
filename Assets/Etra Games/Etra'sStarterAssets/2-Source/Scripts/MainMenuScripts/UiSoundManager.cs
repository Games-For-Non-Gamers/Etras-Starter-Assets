using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSoundManager : MonoBehaviour
{
    [HideInInspector]public static UiSoundManager I;
    [HideInInspector] public AudioManager a;
    private void Awake()
    {
        //Set up Instance so it can be easily referenced. 
        if (I == null)
        {
            I = this;
        }
        else if (I != this)
        {
            I = this;
        }

        a = GetComponent<AudioManager>();
    }

}
