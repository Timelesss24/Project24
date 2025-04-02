using System;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

namespace Timelesss
{
    public class Equipment : MonoBehaviour, IBind<EquipmentData>
    {
        [SerializeField] List<ItemDetails> startingItems = new(); // 초기 장비 아이템
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // 고유 ID

        public EquipmentController Controller { get; private set; }

        void Awake()
        {
            var visualHandler = GetComponent<EquipmentVisualHandler>();
            
            Controller = new EquipmentController.Builder()
                .WithStartingItems(startingItems)
                .WithVisualHandler(visualHandler)
                .Build();
        }

        public void Bind(EquipmentData data)
        {
            Controller.Bind(data);
            data.Id = Id;
        }

        void OnGUI()
        {
            if (Controller == null || Controller.Model == null || Controller.Model.equipmentData == null) return;
        
            GUILayout.Label($"Equipment Data ID: {Controller.Model.equipmentData.Id.ToGuid()}");
        
            foreach (var item in Controller.Model.equipmentData.Items)
            {
                if (item != null)
                {
                    GUILayout.Label($"Item Name: {item.Details?.Name}, Type: {item.Details?.EquipmentType}");
                }
                else
                {
                    GUILayout.Label("Empty Slot");
                }
            }
        }
    }
}