using System;
using UnityEngine;

namespace Timelesss
{
    [Serializable]
    public class Item
    {
        [field: SerializeField] public SerializableGuid Id;
        [field: SerializeField] public SerializableGuid DetailsId;
        public ItemDetails Details;
        public event Action OnChanged = delegate { };
        [SerializeField] public int quantity;

         public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnChanged?.Invoke(); // 변화 알림
                }
            }
        }

        public Item(ItemDetails details, int quantity = 1)
        {
            Id = SerializableGuid.NewGuid();
//            Debug.Log("new Item "+Id.ToGuid());
            DetailsId = details.Id;
            Details = details;
            Quantity = quantity;
        }
    }

    [Serializable]
    public class EquipmentItem : Item
    {
        public EquipmentType EquipmentType => Details.EquipmentType;
        public GameObject EquipmentPrefab => Details.EquipmentPrefab;
        public Vector3 LocalPosition => Details.LocalPosition;
        public Vector3 LocalRotation => Details.LocalRotation;

        public EquipmentItem(ItemDetails details, int quantity = 1) : base(details, quantity)
        {
        }
    }

    [Serializable]
    public class ConsumableItem : Item
    {
        public int RestoreAmount => Details.RestoreValue;

        public ConsumableItem(ItemDetails details, int quantity = 1) : base(details, quantity)
        {
        }
    }

    // [Serializable]
    // public class Item {
    //     [field: SerializeField] public SerializableGuid Id;
    //     [field: SerializeField] public SerializableGuid detailsId; 
    //     public ItemDetails details;
    //     public int quantity;
    //
    //     public EquipmentType EquipmentType;
    //     
    //     
    //     public bool IsEquipment => details.ItemType == ItemType.Equipment;
    //     
    //     public Item(ItemDetails details, int quantity = 1) {
    //         Id = SerializableGuid.NewGuid();
    //         this.detailsId = details.Id;
    //         this.details = details;
    //         this.quantity = quantity;
    //         EquipmentType = details.EquipmentType;
    //     }
    // }
}