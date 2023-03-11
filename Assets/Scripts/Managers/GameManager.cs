using System.Collections.Generic;
using Enemies;
using Items;
using Maps;
using Player;
using Audio;
using UnityEngine;
using Grid = Maps.Grid<Maps.GridCell<bool>>;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        #region SingleTon

        public static GameManager Instance { get; private set; }

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

        [HideInInspector] public AudioManager audioManager;

        [Header("Prefabs")] public GameObject playerPrefab;
        public GameObject chaserPrefab;
        public GameObject runnerPrefab;
        public GameObject trapPrefab;
        public GameObject coinPrefab;
        public GameObject portalPrefab;
        
        public LayerMask spawnables;
        private AtticGenerator _atticGenerator;

        [Header("Initial values")] public int startingWidth;
        public int startingHeight;
        public int startingCellsToRemove;
        public List<Vector2> gremlinsList;

        [Header("Map changes per level")] public int widthIncrease;
        public int heightIncrease;
        public int cellsToRemoveIncrease;
        public int turnsToIncrease;

        [Header("Shop Prices")] public int coffeePrice;
        public int trapPrice;
        public int pillsPrice;
        public int armorPrice;
        public int energyPrice;

        [Header("Upgrade Values")] 
        [Range(0,1)] public float coffeeRecoveryPercentage;
        [Range(0,1)] public float[] armorDamageDecreasePercentage;
        public int energyRoundsIncrease;
        
        public int CurrentArmorUpgrades { get; private set; }
        private int _currentPillsUpgrades;

        [Header("UI Elements")] public GameObject hudUI;
        public GameObject shopUI;
        public bool ShopIsOpen { get; private set; }
        //BTW Pedro, isto é só para dar enable, disable da HUD.
        //quando fizeres o codigo da HUD, melhor fazeres uma classe para isso na pasta UI
        //tipo HUDManager;
        
        
        private int _currentLevel;
        private int _currentWidth;
        private int _currentHeight;
        private int _currentCellsToRemove;
        private Vector3 _playerStartPosition;
        private bool _spawnedEndPortal;
        private List<GameObject> _spawnedTrapsAndCoins;

        private void Start()
        {
            _atticGenerator = GetComponent<AtticGenerator>();
            _currentLevel = 1;
            _currentWidth = startingWidth;
            _currentHeight = startingHeight;
            _currentCellsToRemove = startingCellsToRemove;
            CurrentArmorUpgrades = 0;
            _spawnedTrapsAndCoins = new List<GameObject>();
            StartLevel();

            audioManager = GetComponent<AudioManager>();
            audioManager.Play("Music");
        }

        private void StartLevel()
        {
            _atticGenerator.Generate(_currentWidth, _currentHeight, _currentCellsToRemove);
            Grid g = _atticGenerator.Attic.Grid;
            for (int i = 0; i < g.Cells.Length; i++)
            { 
                GridCell<bool> cell = g.Get(i);
                Vector2Int coordinates = g.GetCoordinates(cell);
                Vector3 playerPos = new Vector3(coordinates.x, coordinates.y, 0);
                
                if (!cell.Value)
                {
                    if (PlayerEntity.Instance == null)
                    {
                        Instantiate(playerPrefab, playerPos, Quaternion.identity);
                    }
                    PlayerEntity.Instance.gameObject.transform.position = playerPos;
                    _playerStartPosition = playerPos;
                    break;
                }
            }

            int chasersSpawned = 0, runnersSpawned = 0, trapsSpawned = 0, coinsSpawned = 0;
            while (chasersSpawned < gremlinsList[_currentLevel-1].y)
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition && Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    GameObject chaser = Instantiate(chaserPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddGremlin(chaser.GetComponent<Gremlin>());
                    chasersSpawned++;
                }
            }
            while (runnersSpawned < gremlinsList[_currentLevel-1].x)
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition && Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    GameObject runner = Instantiate(runnerPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddGremlin(runner.GetComponent<Gremlin>());
                    runnersSpawned++;
                }
            }

            while (trapsSpawned < gremlinsList[_currentLevel-1].y + 1)
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition && Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    GameObject trap = Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddTrap(trap.GetComponent<Trap>());
                    _spawnedTrapsAndCoins.Add(trap);
                    trapsSpawned++;
                }
            }
            
            while (coinsSpawned < Mathf.Floor(gremlinsList[_currentLevel-1].x + 1.5f * gremlinsList[_currentLevel-1].y))
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition && Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
                    coinsSpawned++;
                    _spawnedTrapsAndCoins.Add(coin);

                }
            }

            for (int i = 0; i < _currentPillsUpgrades; i++)
            {
                TurnManager.Instance.RemoveRandomGremlin();
            }

            _currentLevel++;
            if (_currentLevel - 1 % turnsToIncrease == 0)
            {
                _currentWidth += widthIncrease;
                _currentHeight += heightIncrease;
                _currentCellsToRemove += cellsToRemoveIncrease;
            }
        }

        public void SpawnEndPortal()
        {
            if (!_spawnedEndPortal)
            {
                GameObject portal = Instantiate(portalPrefab, _playerStartPosition, Quaternion.identity);
                TurnManager.Instance.portalInMap = portal.GetComponent<EndPortal>();
            }
        }

        public void OpenShop()
        {
            foreach (GameObject item in _spawnedTrapsAndCoins)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _spawnedTrapsAndCoins.Clear();
            _currentPillsUpgrades = 0;
            PlayerEntity.Instance.traps.ResetTraps();
            hudUI.SetActive(false);
            shopUI.SetActive(true);
            ShopIsOpen = true;
            StartLevel();
        }

        public void CloseShop()
        {
            shopUI.SetActive(false);
            hudUI.SetActive(true);
            ShopIsOpen = false;
            _spawnedEndPortal = false;
            PlayerEntity.Instance.gameObject.transform.position = _playerStartPosition;
        }

        public void BuyCoffee()
        {
            PlayerEntity.Instance.inventory.SpendGold(coffeePrice);
            PlayerEntity.Instance.health.RestoreHealth( 
                Mathf.RoundToInt(PlayerEntity.Instance.health.maxHealth * coffeeRecoveryPercentage));
        }

        public void BuyTrap()
        {
            PlayerEntity.Instance.inventory.SpendGold(trapPrice);
            PlayerEntity.Instance.traps.AddTrap();
        }

        public void BuyPills()
        {
            PlayerEntity.Instance.inventory.SpendGold(pillsPrice);
            _currentPillsUpgrades++;
        }

        public void BuyArmor()
        {
            PlayerEntity.Instance.inventory.SpendGold(armorPrice);
            CurrentArmorUpgrades++;
        }

        public void BuyEnergy()
        {
            PlayerEntity.Instance.inventory.SpendGold(energyPrice);
            TurnManager.Instance.turnsForSleepDrop += energyRoundsIncrease;
        }
    }
}