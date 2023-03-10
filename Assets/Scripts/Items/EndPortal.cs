using Extensions;
using Managers;
using Player;
using UnityEngine;

namespace Items
{
    public class EndPortal : MonoBehaviour
    {
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