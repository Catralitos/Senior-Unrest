using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        #region SingleTon

        public static PlayerManager Instance { get; private set; }

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

        [HideInInspector] public PlayerHealth health;
        [HideInInspector] public PlayerMovement movement;
        [HideInInspector] public PlayerTraps traps;
        [HideInInspector] public PlayerCatch catcher;

        private void Start()
        {
            health = GetComponent<PlayerHealth>();
            movement = GetComponent<PlayerMovement>();
            traps = GetComponent<PlayerTraps>();
            catcher = GetComponent<PlayerCatch>();
        }
    }
}