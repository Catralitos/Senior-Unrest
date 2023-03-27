using System.Linq;
using Managers;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// The class that manages the player picking up and laying traps
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class PlayerTraps : MonoBehaviour
    {
        /// <summary>
        /// How many traps the player carries
        /// </summary>
        public int trapSlots;
        /// <summary>
        /// The current traps.
        /// There are simpler ways to do this
        /// But this is a remnant when each key 1-5 would hold a trap,
        /// And picking up/placing the trap would use those buttons
        /// We switched it to one button, but instead of just using a queue or ints
        /// I kept this bool[] system so I didn't have waste jam time remaking code.
        /// </summary>
        [HideInInspector] public bool[] currentTraps;
        /// <summary>
        /// The traps layer
        /// </summary>
        public LayerMask traps;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            currentTraps = new bool[trapSlots];
            for (var i = 0; i < currentTraps.Length; i++) currentTraps[i] = false;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            //If the player presses the button, they will pick up or place a trap.
            if (!TurnManager.Instance.CanMove()) return;
            if (Input.GetKeyDown(KeyCode.E))
                MoveTrap();
        }

        /// <summary>
        /// Moves the trap.
        /// </summary>
        private void MoveTrap()
        {
            //First we check if there is a trap at the location
            Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(0.5f, 0.5f), 0, traps);
            //If so, we pick it up.
            if (col != null && CurrentAmountOfTraps() < trapSlots)
            {
                TurnManager.Instance.PickUpTrap(col.gameObject);
                currentTraps[FirstFreeIndex()] = true;
                TurnManager.Instance.ProcessTurn(transform.position);
            }
            //Otherwise, we place one.
            else if (col == null && CurrentAmountOfTraps() > 0)
            {
                TurnManager.Instance.PlaceTrap(transform.position);
                currentTraps[LastFullIndex()] = false;
                TurnManager.Instance.ProcessTurn(transform.position);
            }
        }

        /// <summary>
        /// Adds a trap.
        /// </summary>
        public void AddTrap()
        {
            currentTraps[FirstFreeIndex()] = true;
        }

        /// <summary>
        /// Gets current amount of traps.
        /// </summary>
        /// <returns>Current amount of traps.</returns>
        public int CurrentAmountOfTraps()
        {
            return currentTraps.Count(trap => trap);
        }

        /// <summary>
        /// Gets the first free trap slot
        /// </summary>
        /// <returns>First free index in array</returns>
        private int FirstFreeIndex()
        {
            for (var i = 0; i < currentTraps.Length; i++)
                if (!currentTraps[i])
                    return i;
            return -1;
        }

        /// <summary>
        /// Gets the last full trap slot
        /// </summary>
        /// <returns>Last full index in array</returns>
        private int LastFullIndex()
        {
            for (var i = currentTraps.Length - 1; i >= 0; i--)
                if (currentTraps[i])
                    return i;
            return -1;
        }

        /// <summary>
        /// Resets the traps.
        /// </summary>
        public void ResetTraps()
        {
            for (var i = 0; i < currentTraps.Length; i++) currentTraps[i] = false;
        }
    }
}