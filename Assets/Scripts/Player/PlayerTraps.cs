using System.Linq;
using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerTraps : MonoBehaviour
    {

        public int trapSlots;
        [HideInInspector] public bool[] currentTraps;
        public LayerMask traps;
        
        private void Start()
        {
            currentTraps = new bool[trapSlots];
        }

        private void Update()
        {
            if (!TurnManager.Instance.CanMove()) return;
            if (Input.GetKeyDown(KeyCode.E))
                MoveTrap();
            /*if (Input.GetKeyDown(KeyCode.Alpha1))
                MoveTrap(0);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                MoveTrap(1);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                MoveTrap(2);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                MoveTrap(3);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                MoveTrap(4);*/
        }

        private void MoveTrap()
        {
            Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(0.5f, 0.5f), 0, traps);
            if (col != null && CurrentAmountOfTraps() < trapSlots)
            {
                TurnManager.Instance.PickUpTrap(col.gameObject);
                currentTraps[FirstFreeIndex()] = true;
                TurnManager.Instance.ProcessTurn(transform.position);
            } else if (col == null && CurrentAmountOfTraps() > 0)
            {
                TurnManager.Instance.PlaceTrap(transform.position);
                currentTraps[LastFullIndex()] = false;
                TurnManager.Instance.ProcessTurn(transform.position);
            }
        }

        private int CurrentAmountOfTraps()
        {
            return currentTraps.Count(trap => trap);
        }

        private int FirstFreeIndex()
        {
            for (int i = 0; i < currentTraps.Length; i++)
            {
                if (!currentTraps[i]) return i;
            }
            return -1;
        }
        
        private int LastFullIndex()
        {
            for (int i = currentTraps.Length - 1; i >= 0; i--)
            {
                if (currentTraps[i]) return i;
            }
            return -1;
        }

        public void ResetTraps()
        {
            for (int i = 0; i < currentTraps.Length; i++)
            {
                currentTraps[i] = false;
            }
        }
    }
}