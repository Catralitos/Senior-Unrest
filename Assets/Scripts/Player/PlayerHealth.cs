using System;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {

        public int maxHealth;
        [HideInInspector] public int currentHealth;

        public LayerMask damageables;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public bool CheckDamage()
        {
            Vector3[] fourDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
            
            foreach (Vector3 direction in fourDirections)
            {
                if (Physics2D.OverlapBox(transform.position + direction, new Vector2(1,1), 0, damageables) != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}