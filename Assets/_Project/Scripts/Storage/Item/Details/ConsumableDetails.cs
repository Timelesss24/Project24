using System;
using UnityEngine;

namespace Timelesss
{
    
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
    [Serializable]
    public class ConsumableDetails : ItemDetails
    {
        public int RestoreValue;
        
        public void OnValidate()
        {
            ItemType = ItemType.Consumable;
        }
        
        public override Item Create(int quantity)
        {
            return new ConsumableItem(this, quantity);
        }
    }
}