using Enemies;
using Items;
using Maps;
using Player;
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

            //DontDestroyOnLoad(this.gameObject);
        }

        #endregion

        [Header("Prefabs")] public GameObject playerPrefab;
        public GameObject chaserPrefab;
        public GameObject runnerPrefab;
        public GameObject trapPrefab;
        public GameObject coin;
        
        public LayerMask spawnables;
        private AtticGenerator _atticGenerator;

        [Header("Initial values")] public int startingWidth;
        public int startingHeight;
        public int startingCellsToRemove;
        public int startingRunners;
        public int startingChasers;
        public int startingTraps;
        public int startingCoins;

        [Header("ShopValues")] public int coffeePrice;
        public int trapPrice;
        public int pillsPrice;
        public int armorPrice;
        public int energyPrice;

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
        private int _currentRunners;
        private int _currentChasers;
        private int _currentTraps;
        private int _currentCoins;

        private void Start()
        {
            _atticGenerator = GetComponent<AtticGenerator>();
            _currentLevel = 1;
            _currentWidth = startingWidth;
            _currentHeight = startingHeight;
            _currentCellsToRemove = startingCellsToRemove;
            _currentRunners = startingRunners;
            _currentChasers = startingChasers;
            _currentTraps = startingTraps;
            _currentCoins = startingCoins;
            //TODO tirar isto daqui, não pode ser no start
            StartLevel();
        }

        public void StartLevel()
        {
            _atticGenerator.Generate(_currentWidth, _currentHeight, _currentCellsToRemove);
            Grid g = _atticGenerator.Attic.Grid;
            for (int i = 0; i < g.Cells.Length; i++)
            {
                if (g.Get(i).Value) continue;

                Vector2Int coordinates = g.GetCoordinates(g.Get(i));
                Vector3 playerPos = new Vector3(coordinates.x, coordinates.y, 0);
                if (PlayerEntity.Instance != null)
                {
                    PlayerEntity.Instance.gameObject.transform.position = playerPos;
                }
                else
                {
                    Instantiate(playerPrefab, playerPos, Quaternion.identity);
                }

                break;
            }

            int chasersSpawned = 0, runnersSpawned = 0, trapsSpawned = 0, coinsSpawned = 0;
            while (chasersSpawned < _currentChasers)
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && Physics2D.OverlapBox(spawnPosition, new Vector2(0.5f, 0.5f), 0, spawnables) == null)
                {
                    GameObject chaser = Instantiate(chaserPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddGremlin(chaser.GetComponent<Gremlin>());
                    chasersSpawned++;
                }
            }
            while (runnersSpawned < _currentRunners)
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && Physics2D.OverlapBox(spawnPosition, new Vector2(0.5f, 0.5f), 0, spawnables) == null)
                {
                    GameObject runner = Instantiate(runnerPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddGremlin(runner.GetComponent<Gremlin>());
                    runnersSpawned++;
                }
            }

            while (trapsSpawned < _currentTraps)
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && Physics2D.OverlapBox(spawnPosition, new Vector2(0.5f, 0.5f), 0, spawnables) == null)
                {
                    GameObject trap = Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
                    TurnManager.Instance.AddTrap(trap.GetComponent<Trap>());
                    trapsSpawned++;
                }
            }
            
            while (coinsSpawned < _currentCoins)
            {
                int randomPosition = Random.Range(0,g.Cells.Length);
                GridCell<bool> cell = g.Get(randomPosition);
                Vector2Int cellCoordinates = g.GetCoordinates(cell);
                Vector3 spawnPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0);
                if (!cell.Value && Physics2D.OverlapBox(spawnPosition, new Vector2(0.5f, 0.5f), 0, spawnables) == null)
                {
                    GameObject coin = Instantiate(this.coin, spawnPosition, Quaternion.identity);
                    coinsSpawned++;
                }
            }
        }

        private void OpenShop()
        {
            hudUI.SetActive(false);
            shopUI.SetActive(true);
            ShopIsOpen = true;
        }

        public void CloseShop()
        {
            shopUI.SetActive(false);
            hudUI.SetActive(true);
            ShopIsOpen = false;
        }

        public void BuyCoffee()
        {
            PlayerEntity.Instance.inventory.SpendGold(coffeePrice);
            //PlayerEntity.Instance.health.RestoreHealth();
        }

        public void BuyTrap()
        {
            PlayerEntity.Instance.inventory.SpendGold(trapPrice);
            PlayerEntity.Instance.traps.AddTrap();
        }

        public void BuyPills()
        {
            PlayerEntity.Instance.inventory.SpendGold(pillsPrice);
        }

        public void BuyArmor()
        {
            PlayerEntity.Instance.inventory.SpendGold(armorPrice);
        }

        public void BuyEnergy()
        {
            PlayerEntity.Instance.inventory.SpendGold(energyPrice);
        }
    }
}