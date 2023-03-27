using Managers;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// A class to allow the player to catch running gremlins
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class PlayerCatch : MonoBehaviour
    {
        /// <summary>
        /// The layer of the runners
        /// </summary>
        public LayerMask runners;

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            //If the player can take a turn and presses space, they will attempt to catch a gremlin.
            if (!TurnManager.Instance.CanMove()) return;
            if (Input.GetKeyDown(KeyCode.Space))
                CatchGremlin();
        }

        /// <summary>
        /// Catches the gremlin.
        /// </summary>
        private void CatchGremlin()
        {
            
            Vector3[] fourDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

            //We check each of the four directions to see if there is a gremlin
            foreach (var direction in fourDirections)
            {
                var col = Physics2D.OverlapBox(transform.position + direction, new Vector2(0.5f, 0.5f), 0, runners);
                if (col)
                {
                    //If that is the case, we play the sound, we catch the gremlin in the TurnManager, and also process the turn.
                    PlayerEntity.Instance.audioManager.Play("Attack");
                    PlayerHUD.Instance.AddMessage("Caught a cheeky runner.");
                    TurnManager.Instance.CatchGremlin(col.gameObject);
                    TurnManager.Instance.ProcessTurn(transform.position);
                    PlayerEntity.Instance.animator.SetTrigger("Attack");
                    break;
                }
            }
        }
    }
}