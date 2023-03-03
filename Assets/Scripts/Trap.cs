using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class Trap : MonoBehaviour
{

    [HideInInspector] public bool hasGremlin;
    [HideInInspector] public Gremlin caughtGremlin;
    public LayerMask gremlins;

    public void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Something entered trigger");
        if (gremlins.HasLayer(col.gameObject.layer))
        {
            hasGremlin = true;
            caughtGremlin = col.gameObject.GetComponent<Gremlin>();
        }
    }
}
