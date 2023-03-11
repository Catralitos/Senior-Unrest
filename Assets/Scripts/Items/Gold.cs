using Extensions;
using Player;
using UnityEngine;

namespace Items
{
    public class Gold : MonoBehaviour
    {
        public int value;
        public LayerMask player;
        
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (player.HasLayer(col.gameObject.layer))
            {
                PlayerEntity.Instance.inventory.IncreaseGold(value);
                PlayerHUD.Instance.AddMessage("Picked up " + value + " gold.");
                Destroy(gameObject);
            }
        }
    }
}