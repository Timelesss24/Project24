using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EquipmentModel
    {
        //  public ObservableDictionary<EquipmentType, Item> Items { get; private set; }
        public ObservableArray<Item> Items { get; set; }
        EquipmentData equipmentData = new();

        public event Action<Item[]> OnModelChanged
        {
            add => Items.AnyValueChanged += value;
            remove => Items.AnyValueChanged -= value;
        }

        public EquipmentModel(IEnumerable<ItemDetails> itemDetails)
        {
            Items = new ObservableArray<Item>(Enum.GetValues(typeof(EquipmentType)).Length);
            foreach (var detail in itemDetails)
            {
                var item = detail.Create(1);
                Items.TryAddAt((int)item.Details.EquipmentType, item);
            }
        }

        public void Bind(EquipmentData data)
        {
            equipmentData = data;

            Debug.Log($"{Enum.GetValues(typeof(EquipmentType)).Length} Equipment Bind");
            equipmentData.Items = new Item[Enum.GetValues(typeof(EquipmentType)).Length];

            for (int i = 0; i < equipmentData.Items.Length; i++)
            {
                equipmentData.Items[i] = null;
            }

            Items.Items = equipmentData.Items;
        }

        public Item Get(EquipmentType type) => Items[(int)type];

        public void Clear()
        {
            Items.Clear();
        }

        public bool Add(Item item) => Items.TryAddAt(((int)item.Details.EquipmentType), item);
        // {
        //     var type = item.Details.EquipmentType;
        //     return Items.TrySet(type, item);
        // }


        public bool Remove(Item item) => Items.TryRemoveAt((int)item.Details.EquipmentType);
        // public bool Remove(Item item)
        // {
        //     var type = item.Details.EquipmentType;
        //     if (Items.ContainsKey(type) && Items[type] == item)
        //     {
        //         return Items.TryRemove(type);
        //     }
        //     return false;
        // }
    }
}