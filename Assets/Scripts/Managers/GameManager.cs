using System.Collections.Generic;
using Audio;
using Enemies;
using Items;
using Maps;
using Player;
using UnityEngine;
using Grid = Maps.Grid<Maps.GridCell<bool>>;

namespace Managers
{
    /// <summary>
    /// The GameManager handles loading scenes, generating new levels and other high-level tasks
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class GameManager : MonoBehaviour
    {
        #region SingleTon

        /// <summary>
        /// Gets the sole instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Awakes this instance (if none already exists).
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        #endregion
        
        /// <summary>
        /// The AudioManager
        /// </summary>
        [HideInInspector] public AudioManager audioManager;

        /// <summary>
        /// The player prefab
        /// </summary>
        [Header("Prefabs")] public GameObject playerPrefab;
        /// <summary>
        /// The chaser prefab
        /// </summary>
        public GameObject chaserPrefab;
        /// <summary>
        /// The runner prefab
        /// </summary>
        public GameObject runnerPrefab;
        /// <summary>
        /// The trap prefab
        /// </summary>
        public GameObject trapPrefab;
        /// <summary>
        /// The coin prefab
        /// </summary>
        public GameObject coinPrefab;
        /// <summary>
        /// The portal prefab
        /// </summary>
        public GameObject portalPrefab;

        /// <summary>
        /// The layers of the objects we spawn
        /// </summary>
        public LayerMask spawnables;

        /// <summary>
        /// The starting width of the level
        /// </summary>
        [Header("Initial values")] public int startingWidth;
        /// <summary>
        /// The starting height of the level
        /// </summary>
        public int startingHeight;
        /// <summary>
        /// The starting cells to remove from the level
        /// </summary>
        public int startingCellsToRemove;
        /// <summary>
        /// A list of tuples representing the number of gremlins in each level
        /// </summary>
        public List<Vector2> gremlinsList;

        /// <summary>
        /// The width increase when the level expands
        /// </summary>
        [Header("Map changes per level")] public int widthIncrease;
        /// <summary>
        /// The height increase when the level expands
        /// </summary>
        public int heightIncrease;
        /// <summary>
        /// The cells to remove increase when the level expands
        /// </summary>
        public int cellsToRemoveIncrease;
        /// <summary>
        /// The turns required to expand the level
        /// </summary>
        public int turnsToIncrease;

        /// <summary>
        /// The coffee price
        /// </summary>
        [Header("Shop Prices")] public int coffeePrice;
        /// <summary>
        /// The trap price
        /// </summary>
        public int trapPrice;
        /// <summary>
        /// The pills price
        /// </summary>
        public int pillsPrice;
        /// <summary>
        /// The armor price
        /// </summary>
        public int armorPrice;
        /// <summary>
        /// The energy price
        /// </summary>
        public int energyPrice;

        /// <summary>
        /// The coffee recovery percentage
        /// </summary>
        [Header("Upgrade Values")] [Range(0, 1)]
        public float coffeeRecoveryPercentage;

        /// <summary>
        /// The armor damage decrease percentage
        /// </summary>
        [Range(0, 1)] public float[] armorDamageDecreasePercentage;
        /// <summary>
        /// The energy rounds increase
        /// </summary>
        public int energyRoundsIncrease;

        /// <summary>
        /// The hud UI
        /// </summary>
        [Header("UI Elements")] public GameObject hudUI;
        /// <summary>
        /// The shop UI
        /// </summary>
        public GameObject shopUI;
        /// <summary>
        /// The attic generator
        /// </summary>
        private AtticGenerator _atticGenerator;
        /// <summary>
        /// The current cells to remove
        /// </summary>
        private int _currentCellsToRemove;
        /// <summary>
        /// The current height
        /// </summary>
        private int _currentHeight;
        /// <summary>
        /// The current width
        /// </summary>
        private int _currentWidth;
        /// <summary>
        /// The player start position
        /// </summary>
        private Vector3 _playerStartPosition;
        /// <summary>
        /// The spawned end portal
        /// </summary>
        private bool _spawnedEndPortal;
        /// <summary>
        /// The spawned traps and coins
        /// </summary>
        private List<GameObject> _spawnedTrapsAndCoins;

        /// <summary>
        /// Gets the current armor upgrades.
        /// </summary>
        /// <value>
        /// The current armor upgrades.
        /// </value>
        public int CurrentArmorUpgrades { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [shop is open].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [shop is open]; otherwise, <c>false</c>.
        /// </value>
        public bool ShopIsOpen { get; private set; }

        /// <summary>
        /// Gets the current level.
        /// </summary>
        /// <value>
        /// The current level.
        /// </value>
        public int CurrentLevel { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            _atticGenerator = GetComponent<AtticGenerator>();
            CurrentLevel = 1;
            _currentWidth = startingWidth;
            _currentHeight = startingHeight;
            _currentCellsToRemove = startingCellsToRemove;
            CurrentArmorUpgrades = 0;
            _spawnedTrapsAndCoins = new List<GameObject>();
            
            //On Start, we create the first level, and start playing music
            StartLevel();

            audioManager = GetComponent<AudioManager>();
            audioManager.Play("Music");
        }

        /// <summary>
        /// Starts the level.
        /// </summary>
        private void StartLevel()
        {
            //Generate the map through the attic generator
            _atticGenerator.Generate(_currentWidth, _currentHeight, _currentCellsToRemove);
            var g = _atticGenerator.Attic.Grid;
            
            //Place the player on the first free cell
            for (var i = 0; i < g.Cells.Length; i++)
            {
                var cell = g.Get(i);
                var coordinates = g.GetCoordinates(cell);
                var playerPos = new Vector3(coordinates.x, coordinates.y, 0);

                if (!cell.Value)
                {
                    if (PlayerEntity.Instance == null) Instantiate(playerPrefab, playerPos, Quaternion.identity);
                    PlayerEntity.Instance.gameObject.transform.position = playerPos;
                    _playerStartPosition = playerPos;
                    break;
                }
            }

            //Spawn each of the enemies/traps/coins of the level in random positions.
            int chasersSpawned = 0, runnersSpawned = 0, trapsSpawned = 0, coinsSpawned = 0;
            while (chasersSpawned < gremlinsList[CurrentLevel - 1].y)
            {
                var randomPosition = Random.Range(0, g.Cells.Length);
                var cell = g.Get(randomPosition);
                var cellCoordinates = g.GetCoordinates(cell);
                var spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition &&
                    Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    var chaser = Instantiate(chaserPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddGremlin(chaser.GetComponent<Gremlin>());
                    chasersSpawned++;
                }
            }

            while (runnersSpawned < gremlinsList[CurrentLevel - 1].x)
            {
                var randomPosition = Random.Range(0, g.Cells.Length);
                var cell = g.Get(randomPosition);
                var cellCoordinates = g.GetCoordinates(cell);
                var spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition &&
                    Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    var runner = Instantiate(runnerPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddGremlin(runner.GetComponent<Gremlin>());
                    runnersSpawned++;
                }
            }

            while (trapsSpawned < gremlinsList[CurrentLevel - 1].y + 1)
            {
                var randomPosition = Random.Range(0, g.Cells.Length);
                var cell = g.Get(randomPosition);
                var cellCoordinates = g.GetCoordinates(cell);
                var spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition &&
                    Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    var trap = Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddTrap(trap.GetComponent<Trap>());
                    _spawnedTrapsAndCoins.Add(trap);
                    trapsSpawned++;
                }
            }

            while (coinsSpawned <
                   Mathf.Floor(gremlinsList[CurrentLevel - 1].x + 1.5f * gremlinsList[CurrentLevel - 1].y))
            {
                var randomPosition = Random.Range(0, g.Cells.Length);
                var cell = g.Get(randomPosition);
                var cellCoordinates = g.GetCoordinates(cell);
                var spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && spawnPosition != _playerStartPosition &&
                    Physics2D.OverlapBox(spawnPosition, new Vector2(0.75f, 0.75f), 0, spawnables) == null)
                {
                    var coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
                    coinsSpawned++;
                    _spawnedTrapsAndCoins.Add(coin);
                }
            }

            //Increase level and check if the level needs to be changed.
            CurrentLevel++;
            if (CurrentLevel - 1 % turnsToIncrease == 0)
            {
                _currentWidth += widthIncrease;
                _currentHeight += heightIncrease;
                _currentCellsToRemove += cellsToRemoveIncrease;
            }
        }

        /// <summary>
        /// Spawns the end portal.
        /// </summary>
        public void SpawnEndPortal()
        {
            if (!_spawnedEndPortal)
            {
                var portal = Instantiate(portalPrefab, _playerStartPosition, Quaternion.identity);
                TurnManager.Instance.portalInMap = portal.GetComponent<EndPortal>();
            }
        }

        /// <summary>
        /// Opens the shop. Also resets and creates a new level in the background.
        /// </summary>
        public void OpenShop()
        {
            foreach (var item in _spawnedTrapsAndCoins)
                if (item != null)
                    Destroy(item);
            _spawnedTrapsAndCoins.Clear();
            PlayerEntity.Instance.traps.ResetTraps();
            hudUI.SetActive(false);
            shopUI.SetActive(true);
            ShopIsOpen = true;
            StartLevel();
        }

        /// <summary>
        /// Closes the shop and resets player position.
        /// </summary>
        public void CloseShop()
        {
            shopUI.SetActive(false);
            hudUI.SetActive(true);
            ShopIsOpen = false;
            _spawnedEndPortal = false;
            PlayerEntity.Instance.gameObject.transform.position = _playerStartPosition;
        }

        /// <summary>
        /// Buys the coffee.
        /// </summary>
        public void BuyCoffee()
        {
            PlayerEntity.Instance.inventory.SpendGold(coffeePrice);
            PlayerEntity.Instance.health.RestoreHealth(
                Mathf.RoundToInt(PlayerEntity.Instance.health.maxHealth * coffeeRecoveryPercentage));
        }

        /// <summary>
        /// Buys the trap.
        /// </summary>
        public void BuyTrap()
        {
            PlayerEntity.Instance.inventory.SpendGold(trapPrice);
            PlayerEntity.Instance.traps.AddTrap();
        }

        /// <summary>
        /// Buys the pills.
        /// </summary>
        public void BuyPills()
        {
            PlayerEntity.Instance.inventory.SpendGold(pillsPrice);
            TurnManager.Instance.RemoveRandomGremlin();
        }

        /// <summary>
        /// Buys the armor.
        /// </summary>
        public void BuyArmor()
        {
            PlayerEntity.Instance.inventory.SpendGold(armorPrice);
            CurrentArmorUpgrades++;
        }

        /// <summary>
        /// Buys the energy.
        /// </summary>
        public void BuyEnergy()
        {
            PlayerEntity.Instance.inventory.SpendGold(energyPrice);
            TurnManager.Instance.turnsForSleepDrop += energyRoundsIncrease;
        }
    }
}