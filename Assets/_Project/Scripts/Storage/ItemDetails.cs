using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    [Serializable]
    public class ItemDetails : ScriptableObject
    {
        public string Name;
        public int MaxStack = 1;
        public SerializableGuid Id = SerializableGuid.NewGuid();
        
        public ItemType ItemType;
        public EquipmentType EquipmentType;

        void AssignNewGuid()
        {
            Id = SerializableGuid.NewGuid();
        }

        public Sprite Icon;

        [TextArea]
        public string Description;

        public Item Create(int quantity)
        {
            return new Item(this, quantity);
        }
    }
}