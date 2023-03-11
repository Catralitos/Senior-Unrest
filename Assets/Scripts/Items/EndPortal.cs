using Extensions;
using Managers;
using Player;
using UnityEngine;
using Audio;

namespace Items
{
    public class EndPortal : MonoBehaviour
    {
        [HideInInspector] public bool hasPlayer;
        [HideInInspector] public AudioManager audioManager;
        public LayerMask player;

        private void Start()
        {
            audioManager = GetComponent<AudioManager>();
            audioManager.Play("Portal");
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            
            if (player.HasLayer(col.gameObject.layer))
            {
                audioManager.Play("enterPortal");
                GameManager.Instance.OpenShop();
                Destroy(gameObject);             
            }
        }
        
    }
}