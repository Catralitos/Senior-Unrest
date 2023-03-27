using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// The class that manages the shop
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class ShopManager : MonoBehaviour
    {
        /// <summary>
        /// The player's current coins
        /// </summary>
        public TextMeshProUGUI currentCoins;

        /// <summary>
        /// The "buy coffee" button
        /// </summary>
        [Header("Buttons")] public Button buyCoffee;
        /// <summary>
        /// The "buy trap" button 
        /// </summary>
        public Button buyTrap;
        /// <summary>
        /// The "buy pills" button
        /// </summary>
        public Button buyPills;
        /// <summary>
        /// The "buy armor" button
        /// </summary>
        public Button buyArmor;
        /// <summary>
        /// The "buy energy drink" button
        /// </summary>
        public Button buyEnergy;
        /// <summary>
        /// The exit shop button
        /// </summary>
        public Button exitButton;

        /// <summary>
        /// The coffee price
        /// </summary>
        [Header("Prices")] public TextMeshProUGUI coffeePrice;
        /// <summary>
        /// The trap price
        /// </summary>
        public TextMeshProUGUI trapPrice;
        /// <summary>
        /// The pills price
        /// </summary>
        public TextMeshProUGUI pillsPrice;
        /// <summary>
        /// The armor price
        /// </summary>
        public TextMeshProUGUI armorPrice;
        /// <summary>
        /// The energy drink price
        /// </summary>
        public TextMeshProUGUI energyPrice;

        /// <summary>
        /// The coffee description
        /// </summary>
        [Header("Descriptions")] public TextMeshProUGUI coffeeDescription;
        /// <summary>
        /// The armor description
        /// </summary>
        public TextMeshProUGUI armorDescription;
        /// <summary>
        /// The energy description
        /// </summary>
        public TextMeshProUGUI energyDescription;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            buyCoffee.onClick.AddListener(BuyCoffee);
            buyTrap.onClick.AddListener(BuyTrap);
            buyPills.onClick.AddListener(BuyPills);
            buyArmor.onClick.AddListener(BuyArmor);
            buyEnergy.onClick.AddListener(BuyEnergy);
            exitButton.onClick.AddListener(CloseShop);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            //Each frame we need to update the price and availability of items, according to the game design
            //We also have the armor values and such be parameters in the game manager, or change over the game
            //So we made the text dynamic to avoid forgetting to change the UI.
            var currentGold = PlayerEntity.Instance.inventory.CurrentGold;
            currentCoins.text = currentGold.ToString();
            buyCoffee.interactable = currentGold >= GameManager.Instance.coffeePrice &&
                                     PlayerEntity.Instance.health.currentHealth <
                                     PlayerEntity.Instance.health.maxHealth;
            buyTrap.interactable = currentGold >= GameManager.Instance.trapPrice &&
                                   PlayerEntity.Instance.traps.CurrentAmountOfTraps() <
                                   PlayerEntity.Instance.traps.trapSlots;
            buyPills.interactable = currentGold >= GameManager.Instance.pillsPrice &&
                                    TurnManager.Instance.GetNumberOfGremlins() > 0;
            buyArmor.interactable = currentGold >= GameManager.Instance.armorPrice
                                    && GameManager.Instance.CurrentArmorUpgrades <
                                    GameManager.Instance.armorDamageDecreasePercentage.Length - 1;
            buyEnergy.interactable = currentGold >= GameManager.Instance.energyPrice;
            coffeeDescription.text = "Restores " + GameManager.Instance.coffeeRecoveryPercentage * 100 +
                                     "% of your max energy.";

            if (GameManager.Instance.CurrentArmorUpgrades <
                GameManager.Instance.armorDamageDecreasePercentage.Length - 1)
                armorDescription.text = "Reduces gremlin damage by " +
                                        (100 - GameManager.Instance.armorDamageDecreasePercentage[
                                            GameManager.Instance.CurrentArmorUpgrades + 1] * 100) + "%.";
            else
                armorDescription.text = "No more armor available.";

            energyDescription.text = "Increases the number of turns before becoming sleepier by " +
                                     GameManager.Instance.energyRoundsIncrease + ".";
        }

        /// <summary>
        /// Called when [enable].
        /// </summary>
        private void OnEnable()
        {
            coffeePrice.text = GameManager.Instance.coffeePrice.ToString();
            trapPrice.text = GameManager.Instance.trapPrice.ToString();
            pillsPrice.text = GameManager.Instance.pillsPrice.ToString();
            armorPrice.text = GameManager.Instance.armorPrice.ToString();
            energyPrice.text = GameManager.Instance.energyPrice.ToString();
        }

        /// <summary>
        /// Buys the coffee.
        /// </summary>
        private static void BuyCoffee()
        {
            GameManager.Instance.BuyCoffee();
        }

        /// <summary>
        /// Buys the trap.
        /// </summary>
        private static void BuyTrap()
        {
            GameManager.Instance.BuyTrap();
        }

        /// <summary>
        /// Buys the pills.
        /// </summary>
        private static void BuyPills()
        {
            GameManager.Instance.BuyPills();
        }

        /// <summary>
        /// Buys the armor.
        /// </summary>
        private void BuyArmor()
        {
            GameManager.Instance.BuyArmor();
            armorDescription.text = "Reduces gremlin damage by " +
                                    (100 - GameManager.Instance.armorDamageDecreasePercentage[
                                        GameManager.Instance.CurrentArmorUpgrades + 1] * 100) + "%.";
        }

        /// <summary>
        /// Buys the energy.
        /// </summary>
        private static void BuyEnergy()
        {
            GameManager.Instance.BuyEnergy();
        }

        /// <summary>
        /// Closes the shop.
        /// </summary>
        private static void CloseShop()
        {
            GameManager.Instance.CloseShop();
        }
    }
}