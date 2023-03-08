using Enemies;
using Extensions;
using UnityEngine;

namespace Items
{
    public class Trap : MonoBehaviour
    {

        [HideInInspector] public bool hasGremlin;
        [HideInInspector] public Gremlin caughtGremlin;
        public LayerMask gremlins;

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (gremlins.HasLayer(col.gameObject.layer))
            {
                hasGremlin = true;
                caughtGremlin = col.gameObject.GetComponent<Gremlin>();
            }
        }
    }
}
