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
                Debug.Log(item.Details.EquipmentType);
                Debug.Log(Items.TryAddAt((int)item.Details.EquipmentType, item));
                Debug.Log(Get(item.Details.EquipmentType));
                
            }
        }

        public void Bind(EquipmentData data)
        {
            for (int i=0; i<Items.Length; i++)
            {
                Debug.Log($"1 {i} {Items[i] != null}");
            }
            
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
            // else {
            //     for (var i = 0; i < equipmentData.Items.Length; i++) {
            //         if (Items[i] == null) continue;
            //         equipmentData.Items[i].Details = ItemDatabase.GetDetailsById(Items[i].DetailsId);
            //     }
            // }
            
           
            if (isNew && Items.Count != 0) {
                Debug.Log(Items.Count);
                
                for (var i = 0; i < Items.Length; i++) {
                    if (Items[i] == null)
                    {
                        Debug.Log(i);
                        continue;
                    }
                    
                    equipmentData.Items[i] = Items[i];
                }
            }
            for (int i=0; i<Items.Length; i++)
            {
                Debug.Log($"2 {i} {Items[i] != null}");
            }


            Items.Items = equipmentData.Items;
            for (int i=0; i<Items.Length; i++)
            {
                Debug.Log($"3 {i} {Items[i] != null}");
            }

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