using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    /// <summary>
    /// Our enemy in the game, who can chase or run away from the player
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class Gremlin : MonoBehaviour
    {
        /// <summary>
        /// The AudioManager
        /// </summary>
        [HideInInspector] public AudioManager audioManager;

        /// <summary>
        /// If the gremlin is a chaser (otherwise, it's a runner)
        /// </summary>
        public bool chaser;
        /// <summary>
        /// The layers that count as obstacles for the gremlines
        /// </summary>
        public LayerMask obstacles;
        /// <summary>
        /// The original position in each turn
        /// </summary>
        private Vector3 _origPos, _targetPos;

        /// <summary>
        /// Stores a value indicating whether this instance is moving.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is moving; otherwise, <c>false</c>.
        /// </value>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// Starts this instance, and saves the AudioManager component from the inspector.
        /// </summary>
        private void Start()
        {
            audioManager = GetComponent<AudioManager>();
        }

        /// <summary>
        /// Moves the gremlin given the player's position
        /// </summary>
        /// <param name="playerPos">The player's position.</param>
        public void Move(Vector3 playerPos)
        {
            //If the gremlin is already moving, return
            if (IsMoving) return;

            //first we check which of the four directions the gremlin can move onto, and put them onto a list
            Vector3[] fourDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

            var possibleMoves = new List<Vector3>();

            foreach (var direction in fourDirections)
                if (Physics2D.OverlapBox(transform.position + direction, new Vector2(0.5f, 0.5f), 0, obstacles) == null)
                {
                    //if player is directly adjacent to gremlin, a.k.a, didn't move
                    if (Math.Abs((transform.position + direction).x - playerPos.x) <= 0.1f &&
                        Math.Abs((transform.position + direction).y - playerPos.y) <= 0.1f)
                    {
                        StartCoroutine(MoveGremlin(Vector3.zero));
                        return;
                    }

                    possibleMoves.Add(direction);
                }

            //Shuffle the list, so if there are ties, it's not just the same direction all the time
            for (var i = 0; i < possibleMoves.Count; i++)
            {
                Vector2 temp = possibleMoves[i];
                var randomIndex = Random.Range(i, possibleMoves.Count);
                possibleMoves[i] = possibleMoves[randomIndex];
                possibleMoves[randomIndex] = temp;
            }

            //Sort the list ascendingly or descendingly, if they are closing the distance, or increasing the distance
            //from the player, respectively
            if (chaser)
                possibleMoves = possibleMoves.OrderBy(x => GetDistanceInTiles(transform.position + x, playerPos))
                    .ToList();
            else
                possibleMoves = possibleMoves
                    .OrderByDescending(x => GetDistanceInTiles(transform.position + x, playerPos)).ToList();

            //Start moving, picking the first item on the list
            StartCoroutine(possibleMoves.Count == 0 ? MoveGremlin(Vector3.zero) : MoveGremlin(possibleMoves[0]));
        }

        /// <summary>
        /// Moves the gremlin.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        private IEnumerator MoveGremlin(Vector3 direction)
        {
            IsMoving = true;

            var elapsedTime = 0.0f;

            _origPos = transform.position;
            _targetPos = _origPos + direction;

            audioManager.Play("Moving");

            //Using a coroutine, we can move the gremlin without teleporting it
            while (elapsedTime < TurnManager.Instance.unitTimeToMove)
            {
                transform.position =
                    Vector3.Lerp(_origPos, _targetPos, elapsedTime / TurnManager.Instance.unitTimeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = _targetPos;

            IsMoving = false;
            yield return null;
        }
        
        /// <summary>
        /// Get a distance in tiles between two position ignoring walls
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        /// <returns>The distance in tiles between two position</returns>
        private static int GetDistanceInTiles(Vector3 pos1, Vector3 pos2)
        {
            return Mathf.RoundToInt(Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y));
        }
    }
}