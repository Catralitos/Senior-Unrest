using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopManager : MonoBehaviour
    {
        public TextMeshProUGUI currentCoins;
    
        [Header("Buttons")] public Button buyCoffee;
        public Button buyTrap;
        public Button buyPills;
        public Button buyArmor;
        public Button buyEnergy;
        public Button exitButton;

        [Header("Prices")] public TextMeshProUGUI coffeePrice;
        public TextMeshProUGUI trapPrice;
        public TextMeshProUGUI pillsPrice;
        public TextMeshProUGUI armorPrice;
        public TextMeshProUGUI energyPrice;

        [Header("Descriptions")] public TextMeshProUGUI coffeeDescription;
        public TextMeshProUGUI armorDescription;
        public TextMeshProUGUI energyDescription;

        private void Start()
        {
            buyCoffee.onClick.AddListener(BuyCoffee);
            buyTrap.onClick.AddListener(BuyTrap);
            buyPills.onClick.AddListener(BuyPills);
            buyArmor.onClick.AddListener(BuyArmor);
            buyEnergy.onClick.AddListener(BuyEnergy);
            exitButton.onClick.AddListener(CloseShop);
        }

        private void OnEnable()
        {
            coffeePrice.text = GameManager.Instance.coffeePrice.ToString();
            trapPrice.text = GameManager.Instance.trapPrice.ToString();
            pillsPrice.text = GameManager.Instance.pillsPrice.ToString();
            armorPrice.text = GameManager.Instance.armorPrice.ToString();
            energyPrice.text = GameManager.Instance.energyPrice.ToString();
        }

        private void Update()
        {
            int currentGold = PlayerEntity.Instance.inventory.currentGold;
            currentCoins.text = currentGold.ToString();
            buyCoffee.interactable = currentGold >= GameManager.Instance.coffeePrice && 
                                     PlayerEntity.Instance.health.currentHealth == PlayerEntity.Instance.health.maxHealth;
            buyTrap.interactable = currentGold >= GameManager.Instance.trapPrice && 
                                   PlayerEntity.Instance.traps.CurrentAmountOfTraps() < PlayerEntity.Instance.traps.trapSlots;
            buyPills.interactable = currentGold >= GameManager.Instance.pillsPrice;
            buyArmor.interactable = currentGold >= GameManager.Instance.armorPrice 
                                    && GameManager.Instance.CurrentArmorUpgrades <  GameManager.Instance.armorDamageDecreasePercentage.Length;
            buyEnergy.interactable = currentGold >= GameManager.Instance.energyPrice;
            coffeeDescription.text = "Restores " + (GameManager.Instance.coffeeRecoveryPercentage * 100) +"% of your max energy.";
            if (buyArmor.interactable)
            {
                armorDescription.text = "Reduces gremlin damage to " +
                                        (GameManager.Instance.armorDamageDecreasePercentage[
                                            GameManager.Instance.CurrentArmorUpgrades + 1] * 100) + "%.";
            }

            energyDescription.text = "Increases the number of turns before becoming sleepier by " +
                                     GameManager.Instance.energyRoundsIncrease + ".";

        }

        private static void BuyCoffee()
        {
            GameManager.Instance.BuyCoffee();
        }

        private static void BuyTrap()
        {
            GameManager.Instance.BuyTrap();
        }

        private static void BuyPills()
        {
            GameManager.Instance.BuyPills();
        }

        private static void BuyArmor()
        {
            GameManager.Instance.BuyArmor();
        }

        private static void BuyEnergy()
        {
            GameManager.Instance.BuyEnergy();
        }

        private static void CloseShop()
        {
            GameManager.Instance.CloseShop();
        }
    }
}
