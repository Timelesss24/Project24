using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class PlayerInventory : MonoBehaviour
    {
        public Queue<ItemData> playerItems;
        Dictionary<EquipType, ItemData> playerEquipSlot;

        private void Awake()
        {
            playerItems = new Queue<ItemData>();
            playerEquipSlot = new Dictionary<EquipType, ItemData>();
        }

        public void DropItemAddToInventory(ItemData itemData)
        {
            playerItems.Enqueue(itemData);
        }

        public void EquipItem(EquipItemData equipItemData)
        {
            if (!playerEquipSlot.ContainsKey(equipItemData.EquipType))
            {
                playerEquipSlot.Add(equipItemData.EquipType, equipItemData);
            }
            else
            {
                playerEquipSlot[equipItemData.EquipType] = equipItemData;
            }
        }
    }
}
