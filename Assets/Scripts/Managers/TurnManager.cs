using System.Collections;
using System.Collections.Generic;
using Enemies;
using Items;
using Player;
using UnityEngine;

namespace Managers
{
    public class TurnManager : MonoBehaviour
    {
    
        #region SingleTon

        public static TurnManager Instance { get; private set; }

        private void Awake()
        {
            // Needed if we want the audio manager to persist through scenes
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
    
        public bool ProcessingTurn { get; private set; }

        public float unitTimeToMove = 0.2f;
    
        public int turnsForSleepDrop;
        private int _currentTurn;
        public int gremlinDamage;
        public int sleepDamage;
    
        [HideInInspector] public int turnsBeforeSleepDrop;
        private List<Gremlin> _enemiesInMap;
        private List<Trap> _trapsInMap;
        [HideInInspector] public EndPortal portalInMap;

        public void Start()
        {
            _enemiesInMap ??= new List<Gremlin>();
            _trapsInMap ??= new List<Trap>();
        }

        public void AddGremlin(Gremlin g)
        {
            _enemiesInMap ??= new List<Gremlin>();
            _enemiesInMap.Add(g);
        }

        public void AddTrap(Trap t)
        {
            _trapsInMap ??= new List<Trap>();
            _trapsInMap.Add(t);
        }

        public void RemoveRandomGremlin()
        {
            int randomIndex = Random.Range(0, _enemiesInMap.Count);
            Gremlin g = _enemiesInMap[randomIndex];
            _enemiesInMap.Remove(g);
            Destroy(g.gameObject);
        }
    
        public void ProcessTurn(Vector3 playerPos)
        {
            if (ProcessingTurn) return;
            ProcessingTurn = true;
            StartCoroutine(TurnCoroutine(playerPos));
        }

        private IEnumerator TurnCoroutine(Vector3 playerPos){
        
            foreach (Gremlin g in _enemiesInMap)
            {
                g.Move(playerPos);
            }

            yield return new WaitForSeconds(unitTimeToMove);
            
            List<Trap> toRemove = new List<Trap>();
            foreach (Trap t in _trapsInMap)
            {
                if (t.hasGremlin)
                {
                    _enemiesInMap.Remove(t.caughtGremlin);
                    Destroy(t.caughtGremlin.gameObject);
                    toRemove.Add(t);
                }
            }
         
            foreach (Trap t in toRemove)
            {
                _trapsInMap.Remove(t);
                Destroy(t.gameObject);
            }

            int surroundingGremlins = PlayerEntity.Instance.health.CheckDamage();
            for (int i = 0; i < surroundingGremlins; i++)
            {
                PlayerEntity.Instance.health.DealDamage(gremlinDamage);
            }
         
            _currentTurn++;
            turnsBeforeSleepDrop++;
            if (turnsBeforeSleepDrop == turnsForSleepDrop)
            {
                PlayerEntity.Instance.health.DealDamage(sleepDamage);
                turnsBeforeSleepDrop = 0;
            }
            
            if (_enemiesInMap.Count==0 && portalInMap == null) GameManager.Instance.SpawnEndPortal();

            if (portalInMap != null)
            {
                if (portalInMap.hasPlayer)
                {
                    GameManager.Instance.OpenShop();
                    GameObject portal = portalInMap.gameObject;
                    portalInMap = null;
                    Destroy(portal);
                }
            }
            
            ProcessingTurn = false;
        }
    
        public void PickUpTrap(GameObject trapObject){
            _trapsInMap.Remove(trapObject.GetComponent<Trap>());
            Destroy(trapObject);
        }
    
        public void PlaceTrap(Vector3 position)
        {
            GameObject instantiatedTrap = Instantiate(GameManager.Instance.trapPrefab, position, Quaternion.identity);
            _trapsInMap.Add(instantiatedTrap.GetComponent<Trap>());
        }

        public void CatchGremlin(GameObject gremlinObject)
        {
            _enemiesInMap.Remove(gremlinObject.GetComponent<Gremlin>());
            Destroy(gremlinObject);
        }

        public void Reset()
        {
            foreach (Gremlin g in _enemiesInMap)
            {
                Destroy(g.gameObject);
            }
            _enemiesInMap.Clear();
        
            foreach (Trap t in _trapsInMap)
            {
                Destroy(t.gameObject);
            }
            _trapsInMap.Clear();
        }
    
        public bool CanMove()
        {
            return !ProcessingTurn && !EntitiesAreMoving() && !GameManager.Instance.ShopIsOpen;
        }

        private bool EntitiesAreMoving()
        {

            foreach (Gremlin g in _enemiesInMap)
            {
                if (g.IsMoving) return true;
            }

            return PlayerEntity.Instance.movement.IsMoving;
        }
    }
}
