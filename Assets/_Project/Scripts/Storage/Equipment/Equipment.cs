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
            Debug.Log($"Equipment {Id.ToGuid()} bound to data {data.Id.ToGuid()}");
        }

        void OnGUI()
        {
            //GUI.Label(new Rect(10, 100, 300, 20), $"Equipment ID: {Id.ToString()}");
            GUILayout.Label($"Inventory ID: {Id.ToGuid()}");
            GUILayout.Label($"Inventory ID: {Controller.Model.equipmentData.Id.ToGuid()}");
            GUILayout.Label($"Inventory ID: {SaveLoadSystem.Instance.GameData.EquipmentData.Id.ToGuid()}");
        }
    }
}