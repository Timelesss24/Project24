using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EquipmentModel
    {
        //  public ObservableDictionary<EquipmentType, Item> Items { get; private set; }
        public ObservableArray<Item> Items { get; set; }
        public EquipmentData equipmentData = new();

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
            }
        }

        public void Bind(EquipmentData data)
        {
            
            for (var i = 0; i < data.Items.Length; i++)
            {
                if (data.Items[i].Id == SerializableGuid.Empty)
                    data.Items[i] = null;
            }
            
            equipmentData = data;

            bool isNew = equipmentData.Items == null || equipmentData.Items.Length == 0;

            if (isNew) {
                equipmentData.Items = new Item[Enum.GetValues(typeof(EquipmentType)).Length];
            }
           
            if (isNew && Items.Count != 0) {
                
                for (var i = 0; i < Items.Length; i++) {
                    if (Items[i] == null) continue;
                    if (Items[i].Quantity == 0) continue;
                    
                    equipmentData.Items[i] = Items[i];
                }
            }


            Items.Items = equipmentData.Items;
        }

        public Item Get(EquipmentType type) => Items[(int)type];

        public void Clear()
        {
            Items.Clear();
        }

        public bool Add(Item item) => Items.TryAddAt(((int)item.Details.EquipmentType), item);


        public bool Remove(Item item) => Items.TryRemoveAt((int)item.Details.EquipmentType);
    }
}