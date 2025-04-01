using System;
using System.Collections.Generic;

namespace Timelesss
{
    public class EquipmentModel
    {
        public ObservableDictionary<EquipmentType, Item> Items { get; private set; }

        EquipmentData equipmentData = new();

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
                Items.TryAdd(item.EquipmentType, item);
            }
        }

        public void Bind(EquipmentData data)
        {
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
                    item.details = ItemDatabase.GetDetailsById(item.detailsId);
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
            var type = item.EquipmentType;
            return Items.TrySet(type, item);
        }

        public bool Remove(Item item)
        {
            var type = item.EquipmentType;
            if (Items.ContainsKey(type) && Items[type] == item)
            {
                return Items.TryRemove(type);
            }
            return false;
        }
    }
}