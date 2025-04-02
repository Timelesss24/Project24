using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EquipmentModel
    {
        public ObservableDictionary<EquipmentType, Item> Items { get; private set; }

        public EquipmentData equipmentData = new();

        public event Action<Dictionary<EquipmentType, Item>> OnModelChanged
        {
            add => Items.AnyValueChanged += value;
            remove => Items.AnyValueChanged -= value;
        }

        public EquipmentModel(IEnumerable<ItemDetails> itemDetails)
        {
            Items = new ObservableDictionary<EquipmentType, Item>();
            foreach (var detail in itemDetails)
            {
                var item = detail.Create(1);
                Items.TryAdd(((EquipmentItem)item).EquipmentType, item);
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

            if (isNew)
            {
                equipmentData.Items = new Item[Enum.GetValues(typeof(EquipmentType)).Length];
            }

            // 데이터 동기화
            foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
            {
                var item = equipmentData.Items[(int)type];

                if (item != null)
                {
                    item  = new EquipmentItem(ItemDatabase.GetDetailsById(item.Id));
                    Items[type] = item;
                }
            }

            // Items와 equipmentData 연결 (역참조)
            foreach (var pair in Items)
            {
                equipmentData.Items[(int)pair.Key] = pair.Value;
            }
            
        }

        public Item Get(EquipmentType type) => Items.GetValueOrDefault(type);

        public void Clear()
        {
            Items.Clear();
        }

        public bool Add(Item item)
        {
            var type = item.Details.EquipmentType;
            return Items.TrySet(type, item);
        }

        public bool Remove(Item item)
        {
            var type = item.Details.EquipmentType;
            if (Items.ContainsKey(type) && Items[type] == item)
            {
                return Items.TryRemove(type);
            }
            return false;
        }
    }
}