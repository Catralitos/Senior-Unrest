using System;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            PlayerEntity.Instance.audioManager.Play("Damage");
            currentHealth = Math.Clamp(Mathf.RoundToInt(currentHealth - (damage*GameManager.Instance.armorDamageDecreasePercentage[GameManager.Instance.CurrentArmorUpgrades])), 0, maxHealth);
            if (currentHealth == 0)
            {
                /*Invoke("Die", 3);
                PlayerEntity.Instance.audioManager.Play("Damage");
                //play animation of grama falling?*/
                Die();
            }
        }

        public void RestoreHealth(int heal)
        {
            currentHealth = Math.Clamp(currentHealth + heal, 0, maxHealth);
        }
        

        private static void Die()
        {
            SceneManager.LoadScene(2);
        }
    }
}