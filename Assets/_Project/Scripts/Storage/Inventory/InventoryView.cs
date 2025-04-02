using System;
using Systems.Persistence;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Timelesss
{
    public class InventoryView : StorageView, IItemContainer, IBind<InventoryData>
    {
        [SerializeField] string panelName = "인벤토리";
        [SerializeField] GameObject slotPrefab; // 슬롯 프리팹

        [SerializeField] Transform slotsParent; // 슬롯 부모 컨테이너

        //[SerializeField] GameObject ghostIconPrefab; // 고스트 아이콘 프리팹
        [SerializeField] Button closeButton; // 닫기 버튼
        [SerializeField] TextMeshProUGUI inventoryHeader; // 제목 텍스트

        [SerializeField] InventoryData inventoryData = new InventoryData();

        public InventoryController Controller { get; private set; }

        public override void InitializeView(int capacity = 0)
        {
            // 슬롯 배열 초기화
            Slots = new Slot[capacity];
            
            Debug.Log($"{capacity} 캐퍼시티");
            
            // 저장 데이터 초기화
            inventoryData.Capacity = capacity;
            inventoryData.Items = new Item[capacity];

            // 제목 설정
            inventoryHeader.text = panelName;

            // 슬롯 영역 UI 생성
            for (int i = 0; i < capacity; i++)
            {
                // 슬롯 프리팹 생성 및 부모 컨테이너에 추가
                var slotObject = Instantiate(slotPrefab, slotsParent);
                var slotComponent = slotObject.GetComponent<Slot>();

                Slots[i] = slotComponent;
                slotComponent.Initialize(i); // 초기화 (예: 인덱스 설정)
            }


            // 닫기 버튼 이벤트 연결
            closeButton.onClick.AddListener(ClosePopup);

            inventoryData = SaveLoadSystem.Instance.LoadGame("Game").InventoryData;
            
            if (inventoryData.Items.Length == 0)
            {
                inventoryData.Items = new Item[capacity];
                inventoryData.Capacity = capacity;
            }

            bool isEmpty = !Array.Exists(inventoryData.Items, item => item != null);

            foreach (var slot in Slots)
            {
                if (slot == null) continue;

                if (isEmpty)
                {
                    Item item = GetItemFromSlot(slot);
                    
                    if (item == null) continue;
                    
                    slot.item = item;
                    slot.item.Quantity = item.Quantity;
                }
                else
                {
                    slot.item = inventoryData.Items[slot.Index];
                    slot.item.Quantity = inventoryData.Items[slot.Index].Quantity;
                }
            }
        }

        protected override Item GetItemFromSlot(Slot slot)
        {
            return Controller?.Model?.Get(slot.Index); // 인벤토리 기준
        }


        public void Bind(InventoryController controller)
        {
            this.Controller = controller;
        }


        public bool HandleDrop(Slot fromSlot, Slot toSlot, Item item)
        {
            if (item == null || Controller?.Model == null) return true;

            var model = Controller.Model;

            // ✔ 장비창에서 인벤토리로 드래그
            if (fromSlot is EquipmentSlot)
            {
                if (model.Add(item))
                {
                    RefreshSlot(toSlot.Index); // UI 갱신
                    return true;
                }

                return false;
            }

            // ✔ 인벤토리 내부 이동
            int fromIndex = fromSlot.Index;
            int toIndex = toSlot.Index;

            if (fromIndex == toIndex) return false;

            var targetItem = model.Get(toIndex);

            if (targetItem != null && targetItem.Details &&
                targetItem.Details.Name == item.Details.Name &&
                targetItem.Quantity + item.Quantity <= item.Details.MaxStack)
            {
                (Slots[fromIndex].item, Slots[toIndex].item) = (Slots[toIndex].item, Slots[fromIndex].item);
                model.Combine(fromIndex, toIndex);
            }
            else
            {
                (Slots[fromIndex].item, Slots[toIndex].item) = (Slots[toIndex].item, Slots[fromIndex].item);
                model.Swap(fromIndex, toIndex);
            }

   

            RefreshSlot(fromIndex);
            RefreshSlot(toIndex);

            SaveItems();

            return true;
        }

        private void SaveItems()
        {
            foreach (var slot in Slots)
            {
                if (slot.item != null)
                {
                    Debug.Log($"배열 길이 : {inventoryData.Items.Length}");
                    
                    inventoryData.Items[slot.Index] = slot.item;
                    inventoryData.Items[slot.Index].Quantity = slot.item.Quantity;

                    if (inventoryData.Items[slot.Index].Details != null)
                        Debug.Log($"{slot.Index}번 {inventoryData.Items[slot.Index].Details.Name}, {inventoryData.Items[slot.Index].Quantity}개 저장 완료");
                    else
                        Debug.Log($"{slot.Index}번 빈슬롯 저장 완료");
                }
            }
            
            
            SaveLoadSystem.Instance.GameData.InventoryData = inventoryData;
            SaveLoadSystem.Instance.SaveGame();
        }

        void RefreshSlot(int index)
        {
            var item = Controller.Model.Get(index);
            if (item == null || item.Id.Equals(SerializableGuid.Empty))
            {
                Slots[index].Set(SerializableGuid.Empty, null);
            }
            else
            {
                Slots[index].Set(item.Id, item.Details.Icon, item.Quantity);
            }
        }

        public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(InventoryData data)
        {
            inventoryData = data;
            Id = data.Id;

            for (int i = 0; i < Slots.Length; i++)
            {
                Item item = inventoryData.Items[i];
                if (item != null)
                {
                    Slots[i].Set(item.Id, item.Details.Icon, item.Quantity);
                }
                else
                {
                    Slots[i].Set(SerializableGuid.Empty, null);
                }
            }
        }
    }
}