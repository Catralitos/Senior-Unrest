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

        public int CheckDamage()
        {
            int c = 0;
            
            Vector3[] fourDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

            foreach (Vector3 direction in fourDirections)
            {
                if (Physics2D.OverlapBox(transform.position + direction, new Vector2(0.5f,0.5f), 0, damageables) != null)
                {
                    c++;
                }
            }

            return c;
        }

        public void DealDamage(int damage)
        {
            currentHealth = Math.Clamp(currentHealth - damage, 0, maxHealth);
            if (currentHealth == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            //TODO implementar morte a sério
            Destroy(gameObject);
        }
    }
}