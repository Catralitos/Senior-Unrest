using System.Collections;
using System.Collections.Generic;
using Enemies;
using Items;
using Player;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// The TurnManager processes each turn of the game
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class TurnManager : MonoBehaviour
    {
        #region SingleTon

        /// <summary>
        /// Gets the sole instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static TurnManager Instance { get; private set; }

        /// <summary>
        /// Awakes this instance (if none have been created already).
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
        /// The time it takes for a unit to move to a new position
        /// </summary>
        public float unitTimeToMove = 0.2f;

        /// <summary>
        /// The turns before the player takes damage because of lack os sleep
        /// </summary>
        public int turnsForSleepDrop;

        /// <summary>
        /// The damage caused by gremlins
        /// </summary>
        public int gremlinDamage;

        /// <summary>
        /// The damage caused by lack of sleep
        /// </summary>
        public int sleepDamage;

        /// <summary>
        /// The turns left before the player takes damage because of lack os sleep
        /// </summary>
        [HideInInspector] public int turnsBeforeSleepDrop;

        /// <summary>
        /// The portal in map
        /// </summary>
        [HideInInspector] public EndPortal portalInMap;

        /// <summary>
        /// The enemies in map
        /// </summary>
        private List<Gremlin> _enemiesInMap;

        /// <summary>
        /// The traps in map
        /// </summary>
        private List<Trap> _trapsInMap;

        /// <summary>
        /// Gets a value indicating whether [processing turn].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [processing turn]; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessingTurn { get; private set; }

        /// <summary>
        /// Gets the current turn.
        /// </summary>
        /// <value>
        /// The current turn.
        /// </value>
        public int CurrentTurn { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            //Create the lists if they haven't been created yet 
            _enemiesInMap ??= new List<Gremlin>();
            _trapsInMap ??= new List<Trap>();
        }

        /// <summary>
        /// Adds a gremlin to the manager.
        /// </summary>
        /// <param name="g">The gremlin.</param>
        public void AddGremlin(Gremlin g)
        {
            _enemiesInMap ??= new List<Gremlin>();
            _enemiesInMap.Add(g);
        }

        /// <summary>
        /// Adds a trap to the manager.
        /// </summary>
        /// <param name="t">The trap.</param>
        public void AddTrap(Trap t)
        {
            _trapsInMap ??= new List<Trap>();
            _trapsInMap.Add(t);
        }

        /// <summary>
        /// Removes a random gremlin.
        /// </summary>
        public void RemoveRandomGremlin()
        {
            if (GetNumberOfGremlins() == 0) return;
            var randomIndex = Random.Range(0, _enemiesInMap.Count);
            var g = _enemiesInMap[randomIndex];
            _enemiesInMap.Remove(g);
            Destroy(g.gameObject);
        }

        /// <summary>
        /// Gets the current number of gremlins.
        /// </summary>
        /// <returns>Number of Gremlins</returns>
        public int GetNumberOfGremlins()
        {
            return _enemiesInMap.Count;
        }

        /// <summary>
        /// Processes the turn.
        /// </summary>
        /// <param name="playerPos">The player's position.</param>
        public void ProcessTurn(Vector3 playerPos)
        {
            if (ProcessingTurn) return;
            ProcessingTurn = true;
            StartCoroutine(TurnCoroutine(playerPos));
        }

        /// <summary>
        /// A coroutine that performs everything required to conclude a turn
        /// </summary>
        /// <param name="playerPos">The player's position.</param>
        /// <returns></returns>
        private IEnumerator TurnCoroutine(Vector3 playerPos)
        {
            //Move all the gremlins
            foreach (var g in _enemiesInMap) g.Move(playerPos);

            //Wait until they have finished moving
            yield return new WaitForSeconds(unitTimeToMove);

            //Remove caught gremlins and the traps that caught them
            var toRemove = new List<Trap>();
            foreach (var t in _trapsInMap)
                if (t.hasGremlin)
                {
                    var type = t.caughtGremlin.chaser ? "Chaser" : "Runner";
                    var pos = new Vector2Int(Mathf.RoundToInt(t.gameObject.transform.position.x),
                        Mathf.RoundToInt(t.gameObject.transform.position.y));
                    PlayerHUD.Instance.AddMessage("Caught a " + type + " in a trap at " + pos + ".");
                    _enemiesInMap.Remove(t.caughtGremlin);
                    Destroy(t.caughtGremlin.gameObject);
                    toRemove.Add(t);
                }

            foreach (var t in toRemove)
            {
                _trapsInMap.Remove(t);
                Destroy(t.gameObject);
            }

            //Check if there are gremlins around the player and reduce their health accordingly
            var surroundingGremlins = PlayerEntity.Instance.health.CheckDamage();
            for (var i = 0; i < surroundingGremlins; i++)
            {
                PlayerEntity.Instance.health.DealDamage(gremlinDamage);
                PlayerHUD.Instance.AddMessage("Lost " + gremlinDamage + " sleep due to an impudent chaser!");
            }

            //Increase the number of turns, and if the right amount has passed, take damage from lack of sleep
            CurrentTurn++;
            turnsBeforeSleepDrop++;
            if (turnsBeforeSleepDrop == turnsForSleepDrop)
            {
                PlayerEntity.Instance.health.DealDamage(sleepDamage);
                turnsBeforeSleepDrop = 0;
                PlayerHUD.Instance.AddMessage("Lost " + sleepDamage + " sleep because you are very tired.");
            }

            //If all the gremlins are caught, spawn the portal that can end the srage.
            if (_enemiesInMap.Count == 0 && portalInMap == null)
            {
                GameManager.Instance.SpawnEndPortal();
                PlayerHUD.Instance.AddMessage("The level exit has spawned!");
            }

            //If the portal has been spawned and the player stepped into it, open the shop and destroy the portal.
            if (portalInMap != null)
                if (portalInMap.hasPlayer)
                {
                    GameManager.Instance.OpenShop();
                    var portal = portalInMap.gameObject;
                    portalInMap = null;
                    Destroy(portal);
                }

            ProcessingTurn = false;
        }

        /// <summary>
        /// Picks up a trap.
        /// </summary>
        /// <param name="trapObject">The trap object.</param>
        public void PickUpTrap(GameObject trapObject)
        {
            _trapsInMap.Remove(trapObject.GetComponent<Trap>());
            Destroy(trapObject);
        }

        /// <summary>
        /// Places the a trap.
        /// </summary>
        /// <param name="position">The position.</param>
        public void PlaceTrap(Vector3 position)
        {
            var instantiatedTrap = Instantiate(GameManager.Instance.trapPrefab, position, Quaternion.identity);
            _trapsInMap.Add(instantiatedTrap.GetComponent<Trap>());
        }

        /// <summary>
        /// Catches a gremlin.
        /// </summary>
        /// <param name="gremlinObject">The gremlin object.</param>
        public void CatchGremlin(GameObject gremlinObject)
        {
            _enemiesInMap.Remove(gremlinObject.GetComponent<Gremlin>());
            Destroy(gremlinObject);
        }

        /// <summary>
        /// Determines whether this instance can move.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can move; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMove()
        {
            return !ProcessingTurn && !EntitiesAreMoving() && !GameManager.Instance.ShopIsOpen;
        }

        /// <summary>
        /// Check if the player or gremlins are moving
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the player or a gremlin is still moving; otherwise, <c>false</c>.
        /// </returns>
        private bool EntitiesAreMoving()
        {
            foreach (var g in _enemiesInMap)
                if (g.IsMoving)
                    return true;

            return PlayerEntity.Instance.movement.IsMoving;
        }
    }
}