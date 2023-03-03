using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gremlin : MonoBehaviour
{
    
    public bool chaser;
    public LayerMask obstacles;
 
    public float timeToMove = 0.2f;
    private Vector3 _origPos, _targetPos;
    
    public void Move(Vector3 playerPos)
    {
        Vector3[] fourDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        List<Vector3> possibleMoves = new List<Vector3>();

        foreach (Vector3 direction in fourDirections)
        {
            if (Physics2D.OverlapBox(playerPos + direction, new Vector2(1,1), 0, obstacles) == null)
            {
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
            possibleMoves = possibleMoves.OrderBy(x => getDistanceInTiles(transform.position + x, playerPos)).ToList();
        }
        else
        {
            possibleMoves = possibleMoves.OrderByDescending(x => getDistanceInTiles(transform.position + x, playerPos)).ToList();
        }

        StartCoroutine(MoveGremlin(possibleMoves[0]));
    }
    
    private IEnumerator MoveGremlin(Vector3 direction)
    {

        float elapsedTime = 0.0f;

        _origPos = transform.position;
        _targetPos = _origPos + direction;

        while(elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(_origPos, _targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = _targetPos;
        
    }

    private int getDistanceInTiles(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.RoundToInt(Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y));
    }
    
    
    
    
    
    
}
