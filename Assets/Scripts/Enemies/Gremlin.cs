using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;
using Audio;

namespace Enemies
{
    public class Gremlin : MonoBehaviour
    {

        [HideInInspector] public AudioManager audioManager;

        public bool chaser;
        public LayerMask obstacles;
    
        public bool IsMoving { get; private set; }
        private Vector3 _origPos, _targetPos;

        private void Start()
        {
            audioManager = GetComponent<AudioManager>();            
        }

        public void Move(Vector3 playerPos)
        {
            if (IsMoving) return;

            Vector3[] fourDirections = {Vector3.up, Vector3.down, Vector3.left, Vector3.right};

            List<Vector3> possibleMoves = new List<Vector3>();

            foreach (Vector3 direction in fourDirections)
            {
                if (Physics2D.OverlapBox(transform.position + direction, new Vector2(0.5f,0.5f), 0, obstacles) == null)
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
            }
        
            for (int i = 0; i < possibleMoves.Count; i++) {
                Vector2 temp = possibleMoves[i];
                int randomIndex = Random.Range(i, possibleMoves.Count);
                possibleMoves[i] = possibleMoves[randomIndex];
                possibleMoves[randomIndex] = temp;
            }

            if (chaser)
            {
                possibleMoves = possibleMoves.OrderBy(x => GetDistanceInTiles(transform.position + x, playerPos)).ToList();
            }
            else
            {
                possibleMoves = possibleMoves.OrderByDescending(x => GetDistanceInTiles(transform.position + x, playerPos)).ToList();
            }

            StartCoroutine(possibleMoves.Count == 0 ? MoveGremlin(Vector3.zero) : MoveGremlin(possibleMoves[0]));
        }
    
        private IEnumerator MoveGremlin(Vector3 direction)
        {

            IsMoving = true;
        
            float elapsedTime = 0.0f;

            _origPos = transform.position;
            _targetPos = _origPos + direction;
        
            while(elapsedTime < TurnManager.Instance.unitTimeToMove)
            {
                transform.position = Vector3.Lerp(_origPos, _targetPos, (elapsedTime / TurnManager.Instance.unitTimeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = _targetPos;

            IsMoving = false;
            yield return null;

        }

        private static int GetDistanceInTiles(Vector3 pos1, Vector3 pos2)
        {
            return Mathf.RoundToInt(Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y));
        }
    
    
    
    
    
    
    }
}
