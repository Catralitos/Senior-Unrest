using Extensions;
using Managers;
using Player;
using UnityEngine;

namespace Items
{
    public class EndPortal : MonoBehaviour
    {
        [HideInInspector] public bool hasPlayer;
        public LayerMask player;
        
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (player.HasLayer(col.gameObject.layer))
            {
                GameManager.Instance.OpenShop();
                Destroy(gameObject);             
            }
        }
        
    }
}