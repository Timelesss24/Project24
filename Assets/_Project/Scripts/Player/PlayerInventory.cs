using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class PlayerInventory : MonoBehaviour
    {
        public Queue<ItemData> playerItems;

        private void Awake()
        {
            playerItems = new Queue<ItemData>();
        }

        public void DropItemAddToInventory(ItemData itemData)
        {
            playerItems.Enqueue(itemData);
        }
    }
}
