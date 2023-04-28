using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EtrasStarterAssets{
    [RequireComponent(typeof(ABILITY_CheckpointRespawn))]
    [AbilityUsage(EtraCharacterMainController.GameplayTypeFlags.All, AbilityUsage.AbilityTypeFlag.Passive)]
    public class ABILITY_ContinousHealth : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}