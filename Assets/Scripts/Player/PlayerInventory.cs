using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        public int currentGold { get; private set; }
        public int currentArmor { get; private set; }
        public int currentCoffee { get; private set; }

        private void Start()
        {
            currentGold = 0;
            currentArmor = 0;
            currentCoffee = 0;
        }

        public void IncreaseGold(int value)
        {
            currentGold += value;
        }

        public void IncreaseArmor(int value)
        {
            currentArmor += value;
        }

        public void IncreaseCoffee(int value)
        {
            currentCoffee += value;
        }
    }
}