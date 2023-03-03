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

    public void ProcessTurn(Vector3 playerPos)
    {
        foreach (Gremlin g in _enemiesInMap)
        {
            g.Move(playerPos);
        }
    }
}
