using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EtrasStarterAssets
{
    public class Enemy : MonoBehaviour
    {
        //Variables
        public int health = 5;

        //Getter and setter
        public int getHealth()
        {
            return health;
        }

        public void setHealth(int number)
        {
            health = number;
        }
    }
}
