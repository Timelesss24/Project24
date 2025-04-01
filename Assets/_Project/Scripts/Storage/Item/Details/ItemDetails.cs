using System;
using UnityEngine;

namespace Timelesss
{
    public enum ItemType
    {
        Consumable,
        Equipment,
        Quest,
    }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    [Serializable]
    public class ItemDetails : ScriptableObject
    {
        public string Name;
        public int MaxStack = 1;
        public SerializableGuid Id = SerializableGuid.NewGuid();

        public ItemType ItemType;
        public GameObject DropPrefab;
        public EquipmentType EquipmentType;
        
        void AssignNewGuid()
        {
            Id = SerializableGuid.NewGuid();
        }

        public Sprite Icon;

        [TextArea]
        public string Description;

        public virtual Item Create(int quantity)
        {
            return new Item(this, quantity);
        }
    }

}