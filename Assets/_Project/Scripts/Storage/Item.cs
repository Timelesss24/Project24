using System;
using UnityEngine;

namespace Timelesss {
    [Serializable]
    public class Item {
        [field: SerializeField] public SerializableGuid Id;
        [field: SerializeField] public SerializableGuid detailsId; 
        public ItemDetails details;
        public int quantity;

        public EquipmentType EquipmentType;
        
        public bool IsEquipment => details.ItemType == ItemType.EquipableItem;
        
        public Item(ItemDetails details, int quantity = 1) {
            Id = SerializableGuid.NewGuid();
            this.detailsId = details.Id;
            this.details = details;
            this.quantity = quantity;
            EquipmentType = details.EquipmentType;
        }
    }
}