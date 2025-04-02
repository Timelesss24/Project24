using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class InventoryModel
    {
        /// 인벤토리에 저장된 아이템 배열입니다. 
        ObservableArray<Item> Items { get; }

        /// 현재 바인딩된 인벤토리 데이터입니다.
        InventoryData inventoryData = new();

        /// 인벤토리의 최대 용량입니다. 
        readonly int capacity;

        /// <summary>
        /// 보유 중인 코인의 양입니다.
        /// 값은 inventoryData.Coins의 값을 기반으로 읽고 쓰기가 가능합니다.
        /// </summary>
        public int Coins
        {
            get => inventoryData.Coins;
            set => inventoryData.Coins = value;
        }

        /// <summary>
        /// 인벤토리 모델이 변경될 때 호출되는 이벤트입니다.
        /// 아이템 배열이 변경될 때 발생합니다.
        /// </summary>
        public event Action<Item[]> OnModelChanged
        {
            add => Items.AnyValueChanged += value;
            remove => Items.AnyValueChanged -= value;
        }

        /// <summary>
        /// InventoryModel 클래스의 생성자.
        /// 주어진 아이템 세부 정보와 용량으로 인벤토리를 초기화합니다.
        /// </summary>
        /// <param name="itemDetails">처음에 추가할 아이템 정보.</param>
        /// <param name="capacity">인벤토리의 최대 용량.</param>
        public InventoryModel(IEnumerable<ItemDetails> itemDetails, int capacity)
        {
            this.capacity = capacity;
            Items = new ObservableArray<Item>(capacity);
            foreach (var itemDetail in itemDetails)
            {
                Items.TryAdd(itemDetail.Create(1));
            }
        }
        
        // 메서드
        /// <summary>
        /// InventoryData와 모델을 바인딩합니다.
        /// 데이터를 갱신 및 동기화하며, 기존 데이터가 존재하지 않으면 초기화합니다.
        /// </summary>
        /// <param name="data">InventoryData 인스턴스.</param>
        public void Bind(InventoryData data)
        {
            inventoryData = data;
            
            inventoryData.Capacity = capacity;

            bool isNew = inventoryData.Items == null || inventoryData.Items.Length == 0;

            if (isNew)
            {
                // 새로운 데이터일 경우 빈 아이템 배열 초기화
                inventoryData.Items = new Item[capacity];
            }
            else
            {
                // 기존 데이터에서 아이템을 로드
                for (var i = 0; i < capacity; i++)
                {
                    if (Items[i] == null) continue;
                    inventoryData.Items[i] = new Item(ItemDatabase.GetDetailsById(Items[i].DetailsId));
                }
            }

            if (isNew && Items.Count != 0)
            {
                // 아이템 복사
                for (var i = 0; i < capacity; i++)
                {
                    if (Items[i] == null) continue;
                    inventoryData.Items[i] = Items[i];
                }
            }

            // Items 배열과 inventoryData의 아이템 연결
            Items.Items = inventoryData.Items;
        }

        /// <summary>
        /// 코인을 추가합니다.
        /// </summary>
        /// <param name="amount">추가할 코인 수량.</param>
        public void AddCoins(int amount) => Coins += amount;

        /// <summary>
        /// 지정된 인덱스에 해당하는 아이템을 반환합니다.
        /// </summary>
        /// <param name="index">아이템의 인덱스.</param>
        /// <returns>지정된 인덱스의 아이템.</returns>
        public Item Get(int index) => Items[index];

        /// <summary>
        /// 인벤토리를 초기화합니다. 모든 아이템이 제거됩니다.
        /// </summary>
        public void Clear() => Items.Clear();

        /// <summary>
        /// 인벤토리에 아이템을 추가합니다.
        /// </summary>
        /// <param name="item">추가할 아이템.</param>
        /// <returns>아이템 추가 성공 여부.</returns>
        public bool Add(Item item) => Items.TryAdd(item);

        /// <summary>
        /// 인벤토리에서 아이템을 제거합니다.
        /// </summary>
        /// <param name="item">제거할 아이템.</param>
        /// <returns>아이템 제거 성공 여부.</returns>
        public bool Remove(Item item) => Items.TryRemove(item);

        /// <summary>
        /// 두 아이템의 위치를 서로 교환합니다.
        /// </summary>
        /// <param name="source">원본 인덱스.</param>
        /// <param name="target">대상 인덱스.</param>
        public void Swap(int source, int target) => Items.Swap(source, target);

        /// <summary>
        /// 두 아이템을 병합하고 수량을 합산합니다.
        /// </summary>
        /// <param name="source">병합할 원본 인덱스.</param>
        /// <param name="target">병합할 대상 인덱스.</param>
        /// <returns>병합 후 대상 아이템의 총 수량.</returns>
        public int Combine(int source, int target)
        {
            var total = Items[source].Quantity + Items[target].Quantity;
            Items[target].Quantity = total;
            Remove(Items[source]);
            return total;
        }
    }
}