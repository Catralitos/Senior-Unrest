﻿using UnityEngine;

namespace Player
{
    public class PlayerCatch: MonoBehaviour
    {
        public LayerMask runners;
        private void Update()
        {
            if (!TurnManager.Instance.CanMove()) return;
            if (Input.GetKeyDown(KeyCode.Space))
                CatchGremlin();
        }

        private void CatchGremlin()
        {
            Vector3[] fourDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
            
            foreach (Vector3 direction in fourDirections)
            {
                Collider2D col = Physics2D.OverlapBox(transform.position + direction, new Vector2(1, 1), 0, runners);
                if (col)
                {
                    TurnManager.Instance.CatchGremlin(col.gameObject);
                    TurnManager.Instance.ProcessTurn(transform.position);
                }
            }
        }
    }
}