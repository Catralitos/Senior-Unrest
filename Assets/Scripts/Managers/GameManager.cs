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

            DontDestroyOnLoad(this.gameObject);
        }

        #endregion

        [Header("Prefabs")] public GameObject playerPrefab;
        public GameObject chaserPrefab;
        public GameObject runnerPrefab;
        public GameObject trapPrefab;

        public LayerMask spawnables;
        private AtticGenerator _atticGenerator;

        [Header("Initial values")] public int startingWidth;
        public int startingHeight;
        public int startingCellsToRemove;
        public int startingRunners;
        public int startingChasers;
        public int startingTraps;

        private int _currentLevel;
        private int _currentWidth;
        private int _currentHeight;
        private int _currentCellsToRemove;
        private int _currentRunners;
        private int _currentChasers;
        private int _currentTraps;

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
        
            int chasersSpawned = 0, runnersSpawned = 0, trapsSpawned = 0;
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
        }

    }
}