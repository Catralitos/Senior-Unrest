using System;
using System.Collections;
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
    
    public bool ProcessingTurn { get; private set; }

    public float unitTimeToMove = 0.2f;
    
    public int turnsForSleepDrop;
    private int _currentTurn;
    public int gremlinDamage;
    public int sleepDamage;
    
    private int _turnsBeforeSleepDrop;
    private List<Gremlin> _enemiesInMap;
    private List<Trap> _trapsInMap;

    private void Start()
    {
        _enemiesInMap = new List<Gremlin>();
        _trapsInMap = new List<Trap>();
    }

    public void AddGremlin(Gremlin g)
    {
        _enemiesInMap.Add(g);
    }

    public void AddTrap(Trap t)
    {
        _trapsInMap.Add(t);
    }
    
    public void ProcessTurn(Vector3 playerPos)
    {
        if (ProcessingTurn) return;
        ProcessingTurn = true;
        StartCoroutine(TurnCoroutine(playerPos));
    }

    private IEnumerator TurnCoroutine(Vector3 playerPos){
        
            foreach (Gremlin g in _enemiesInMap)
            {
                g.Move(playerPos);
            }

            yield return new WaitForSeconds(unitTimeToMove);
            
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

            int surroundingGremlins = PlayerManager.Instance.health.CheckDamage();
            for (int i = 0; i < surroundingGremlins; i++)
            {
                PlayerManager.Instance.health.DealDamage(gremlinDamage);
            }
         
            _currentTurn++;
            if (_currentTurn == turnsForSleepDrop)
            {
                PlayerManager.Instance.health.DealDamage(sleepDamage);
                _currentTurn = 0;
            }
            
            ProcessingTurn = false;
    }
    
    public void PickUpTrap(GameObject trapObject){
        _trapsInMap.Remove(trapObject.GetComponent<Trap>());
        Destroy(trapObject);
    }
    
    public void PlaceTrap(Vector3 position)
    {
        GameObject instantiatedTrap = Instantiate(GameManager.Instance.trapPrefab, position, Quaternion.identity);
        _trapsInMap.Add(instantiatedTrap.GetComponent<Trap>());
    }

    public void CatchGremlin(GameObject gremlinObject)
    {
        _enemiesInMap.Remove(gremlinObject.GetComponent<Gremlin>());
        Destroy(gremlinObject);
    }

    public void Reset()
    {
        foreach (Gremlin g in _enemiesInMap)
        {
            Destroy(g.gameObject);
        }
        _enemiesInMap.Clear();
        
        foreach (Trap t in _trapsInMap)
        {
            Destroy(t.gameObject);
        }
        _trapsInMap.Clear();
    }

    public bool CanMove()
    {
        return !ProcessingTurn && !EntitiesAreMoving();
    }

    private bool EntitiesAreMoving()
    {

        foreach (Gremlin g in _enemiesInMap)
        {
            if (g.IsMoving) return true;
        }

        return PlayerManager.Instance.movement.IsMoving;
    }
}
