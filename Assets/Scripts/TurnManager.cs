using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    
    #region SingleTon

    public static TurnManager Instance { get; private set; }

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
    
    public int turnsForSleepDrop;

    private int _turnsBeforeSleepDrop;
    public List<Gremlin> _enemiesInMap;
    public List<Trap> _trapsInMap;

    public void ProcessTurn(Vector3 playerPos)
    {
        foreach (Gremlin g in _enemiesInMap)
        {
            g.Move(playerPos);
        }

        List<Trap> toRemove = new List<Trap>();
        foreach (Trap t in _trapsInMap)
        {
            if (t.hasGremlin)
            {
                _enemiesInMap.Remove(t.caughtGremlin);
                Destroy(t.caughtGremlin.gameObject);
                toRemove.Add(t);
            }
        }

        foreach (Trap t in toRemove)
        {
            _trapsInMap.Remove(t);
            Destroy(t.gameObject);
        }
    }

    public bool CanMove()
    {
        foreach (Gremlin g in _enemiesInMap)
        {
            if (g.isMoving) return false;
        }

        return true;
    }
}
