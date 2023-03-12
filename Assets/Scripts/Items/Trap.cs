using System;
using Audio;
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
        private AudioManager _audioManager;

        private void Start()
        {
            _audioManager = GetComponent<AudioManager>();
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (gremlins.HasLayer(col.gameObject.layer))
            {
                _audioManager.Play("Trap");
                hasGremlin = true;
                caughtGremlin = col.gameObject.GetComponent<Gremlin>();
            }
        }
    }
}
