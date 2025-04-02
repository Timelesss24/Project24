using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EquipmentModel
    {
        public ObservableDictionary<EquipmentType, Item> dicItems { get;  }
//        ObservableArray<Item> items { get; }

        public EquipmentData equipmentData = new();

        public event Action<Dictionary<EquipmentType, Item>> OnModelChanged
        {
            add => dicItems.AnyValueChanged += value;
            remove => dicItems.AnyValueChanged -= value;
        }

        public EquipmentModel(IEnumerable<ItemDetails> itemDetails)
        {
            dicItems = new ObservableDictionary<EquipmentType, Item>();
            foreach (var detail in itemDetails)
            {
                var item = detail.Create(1);
                dicItems.TryAdd(((EquipmentItem)item).EquipmentType, item);
            }
        }

        // public void Bind(EquipmentData data)
        // {
        //     equipmentData = data;
        //
        //     Items.Items = equipmentData.Items;
        // }
        
        public void Bind(EquipmentData data)
        {
            for (var i = 0; i < data.Items.Length; i++)
            {
                if (data.Items[i].Id == SerializableGuid.Empty)
                    data.Items[i] = null;
            }

            equipmentData = data;

            // equipmentData.Items의 초기화 확인
            bool isNew = equipmentData.Items == null || equipmentData.Items.Length == 0;

            if (isNew)
            {
                equipmentData.Items = new Item[Enum.GetValues(typeof(EquipmentType)).Length];
            }
            else
            {
                foreach (var item in dicItems)
                {
                    if (item.Value == null) continue;
                    var a = equipmentData.Items[(int)item.Key];
                    Debug.Log((int)item.Key);
                    //equipmentData.Items[(int)item.Key].Details = ItemDatabase.GetDetailsById(item.Value.DetailsId);
                }
            }


            var b = new SerializableGuid(3907552367, 1291227998, 438918075, 2240810988);
            Debug.Log($"GUID = {b.ToGuid()}");


            
            // equipmentData.Items를 기반으로 dicItems 동기화
            // dicItems.Clear(); // 기존 데이터를 비웁니다
            for (int i = 0; i < equipmentData.Items.Length; i++)
            {
                Item item = equipmentData.Items[i];
                if (item != null) // Null 체크
                {
                    dicItems[(EquipmentType)i] = item; // EquipmentType을 Enum으로 변환하여 dicItems에 추가
                }
            }
            
        }


        public Item Get(EquipmentType type) => dicItems.GetValueOrDefault(type);

        public void Clear()
        {
            dicItems.Clear();
        }

        public bool Add(Item item)
        {
            var type = item.Details.EquipmentType;
            return dicItems.TrySet(type, item);
        }

        public bool Remove(Item item)
        {
            var type = item.Details.EquipmentType;
            if (dicItems.ContainsKey(type) && dicItems[type] == item)
            {
                return dicItems.TryRemove(type);
            }
            return false;
        }
    }
}