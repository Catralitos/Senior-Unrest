using UnityEngine;

namespace Player
{
    public class PlayerTraps : MonoBehaviour
    {

        [HideInInspector]public bool[] currentTraps;
        public LayerMask traps;
        
        private void Start()
        {
            currentTraps = new[] { false, false, false, false, false };
        }

        private void Update()
        {
            if (!TurnManager.Instance.CanMove()) return;
            if (Input.GetKeyDown(KeyCode.Alpha1))
                MoveTrap(0);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                MoveTrap(1);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                MoveTrap(2);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                MoveTrap(3);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                MoveTrap(4);
        }

        private void MoveTrap(int index)
        {
            Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(0.5f, 0.5f), 0, traps);
            if (col != null && !currentTraps[index])
            {
                TurnManager.Instance.PickUpTrap(col.gameObject);
                currentTraps[index] = true;
                TurnManager.Instance.ProcessTurn(transform.position);
            } else if (col == null && currentTraps[index])
            {
                TurnManager.Instance.PlaceTrap(transform.position);
                currentTraps[index] = false;
                TurnManager.Instance.ProcessTurn(transform.position);
            }
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