using System.Collections.Generic;
using Player;
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

    public GameObject chaserPrefab;
    public GameObject runnerPrefab;
    public GameObject trapPrefab;
    
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
    
    public void PickUpTrap(GameObject trapObject){
        _trapsInMap.Remove(trapObject.GetComponent<Trap>());
        Destroy(trapObject);
    }
    
    public void PlaceTrap(Vector3 position)
    {
        GameObject instantiatedTrap = Instantiate(trapPrefab, position, Quaternion.identity);
        _trapsInMap.Add(instantiatedTrap.GetComponent<Trap>());
    }

    public void CatchGremlin(GameObject gremlinObject)
    {
        _enemiesInMap.Remove(gremlinObject.GetComponent<Gremlin>());
        Destroy(gremlinObject);
    }
    
    public bool CanMove()
    {
        if (PlayerManager.Instance.movement.IsMoving) return false;
        foreach (Gremlin g in _enemiesInMap)
        {
            if (g.isMoving) return false;
        }

        return true;
    }
}
