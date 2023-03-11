using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHUD : MonoBehaviour
    {
        public TextMeshProUGUI energyText;
        public TextMeshProUGUI trapsText;
        public TextMeshProUGUI goldText;

        public Image turnsLeftImage;
        public TextMeshProUGUI turnsLeftText;

        public List<TextMeshProUGUI> logMessages;
        private Queue<string> _logQueue;
        private bool[] _filledSlots;
        
        #region SingleTon

        public static PlayerHUD Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        private void Start()
        {

            _logQueue = new Queue<string>();

            foreach (TextMeshProUGUI text in logMessages)
            {
                text.text = "";
            }
            
            _filledSlots = new bool[logMessages.Count];
            for (int i = 0; i < _filledSlots.Length; i++)
            {
                _filledSlots[i] = false;
            }

            AddMessage("Hello and welcome, adventurer, to yet another dungeon!"); 
        }

        private void Update()
        {
            energyText.text = PlayerEntity.Instance.health.currentHealth + "/" + PlayerEntity.Instance.health.maxHealth;
            trapsText.text = PlayerEntity.Instance.traps.CurrentAmountOfTraps() + "/" + PlayerEntity.Instance.traps.trapSlots;
            goldText.text = PlayerEntity.Instance.inventory.currentGold.ToString();
            turnsLeftImage.fillAmount = 1.0f -(1.0f * TurnManager.Instance.turnsBeforeSleepDrop / TurnManager.Instance.turnsForSleepDrop);
            turnsLeftText.text = (TurnManager.Instance.turnsForSleepDrop - TurnManager.Instance.turnsBeforeSleepDrop)
                .ToString();
        }

        public void AddMessage(string newMessage) {
            _logQueue.Enqueue("Turn " + TurnManager.Instance.currentTurn+ ": " + newMessage);
            if (_logQueue.ToArray().Length > logMessages.Count)
            {
                _logQueue.Dequeue();
            }
            string[] messages = _logQueue.ToArray();
            for (int i = 0; i < messages.Length; i++)
            {
                logMessages[i].text = messages[i];
            }
        }
        
    }
}