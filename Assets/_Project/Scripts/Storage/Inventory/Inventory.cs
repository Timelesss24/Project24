using System;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

namespace Timelesss
{
    public class Inventory : MonoBehaviour, IBind<InventoryData>
    {
        [SerializeField] int capacity = 20; // 인벤토리 슬롯 수
        [SerializeField] List<ItemDetails> startingItems = new(); // 기본 아이템 목록
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // 고유 ID
        [SerializeField] EventChannel<float> coinChannel; // 코인 관련 이벤트 채널

        public InventoryController Controller { get; private set; }

        void Awake()
        {
            Debug.Log("INVENTORY AWAKE");
            // InventoryController 초기화
            Controller = new InventoryController.Builder()
                .WithStartingItems(startingItems)
                .WithCapacity(capacity)
                .Build();
        }

        public void AddItem(Item item)
        {
            Debug.Log($"Adding item {item.Id.ToGuid()} to inventory {Id.ToGuid()}");
            Debug.Log(Controller.Model.Add(item));
            Controller.SubscribeToItem(item);
        }

        /// <summary>
        /// InventoryData를 컨트롤러에 바인딩
        /// </summary>
        public void Bind(InventoryData data)
        {
            Controller.Bind(data); // 모델 데이터와 컨트롤러 연결
            data.Id = Id; // 데이터 ID를 현재 Inventory ID로 설정
        }

        void OnGUI()
        {
            //GUILayout.Label($"Inventory ID: {Controller.Model.inventoryData.Id.ToGuid()}");
        }
    }
}