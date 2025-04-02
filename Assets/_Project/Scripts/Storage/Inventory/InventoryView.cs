using System;
using Systems.Persistence;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Timelesss
{
    public class InventoryView : StorageView, IItemContainer
    {
        [SerializeField] string panelName = "인벤토리";
        [SerializeField] GameObject slotPrefab; // 슬롯 프리팹

        [SerializeField] Transform slotsParent; // 슬롯 부모 컨테이너

        //[SerializeField] GameObject ghostIconPrefab; // 고스트 아이콘 프리팹
        [SerializeField] Button closeButton; // 닫기 버튼
        [SerializeField] TextMeshProUGUI inventoryHeader; // 제목 텍스트

        public InventoryController Controller { get; private set; }

        public override void InitializeView(int capacity = 0)
        {
            // 슬롯 배열 초기화
            Slots = new Slot[capacity];
            
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
            
            
            if ( targetItem != null && targetItem.Details &&
                 targetItem.Details.Id == item.Details.Id &&
                 targetItem.Quantity + item.Quantity <= item.Details.MaxStack)
            {
                model.Combine(fromIndex, toIndex);
            }
            else
            {
                model.Swap(fromIndex, toIndex);
            }

            RefreshSlot(fromIndex);
            RefreshSlot(toIndex);

            return true;
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
    }
}