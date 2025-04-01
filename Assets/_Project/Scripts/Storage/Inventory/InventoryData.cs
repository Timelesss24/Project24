using System;
using Systems.Persistence;
using UnityEngine;

namespace Timelesss {
    [Serializable]
    public class InventoryData : ISaveable {
        [field: SerializeField] public SerializableGuid Id { get; set; } // 고유 ID
        public Item[] Items; // 인벤토리에 저장된 아이템 목록
        public int Capacity; // 인벤토리 용량
        public int Coins; // 인벤토리가 소유한 코인 수
    }
}