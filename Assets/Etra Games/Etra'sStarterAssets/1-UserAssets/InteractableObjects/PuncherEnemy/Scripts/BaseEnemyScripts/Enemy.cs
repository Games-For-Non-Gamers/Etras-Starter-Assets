using UnityEngine;

//All this enemy class currently incorporates is a health variable
namespace Etra.StarterAssets.Interactables.Enemies
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
