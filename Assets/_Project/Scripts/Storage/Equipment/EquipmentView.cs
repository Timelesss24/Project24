using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public enum EquipmentType
    {
        Weapon,
        Helmet,
        Armor,
        Boots
    }

    public class EquipmentView : StorageView, IItemContainer
    {
        [SerializeField] string panelName = "장비창";
        //[SerializeField] GameObject ghostIconPrefab; // 고스트 아이콘 프리팹
        [SerializeField] Button closeButton; // 닫기 버튼
        [SerializeField] TextMeshProUGUI header; // 제목 텍스트
        
        Dictionary<EquipmentType, EquipmentSlot> slotDict;
        
        public EquipmentController  Controller { get; private set; } 
        
        public override void InitializeView(int capacity = 0)
        {
            // 제목 설정
            header.text = panelName;
            
            slotDict = new Dictionary<EquipmentType, EquipmentSlot>();

            if (Slots is EquipmentSlot[] slots)
            {
                foreach (var slot in slots)
                {
                    slotDict[slot.EquipmentType] = slot;

                    slot.Initialize(slot.EquipmentType, this);
                }
            }
            
            // 닫기 버튼 이벤트 연결
            closeButton.onClick.AddListener(ClosePopup);
        }
        
        protected override Item GetItemFromSlot(Slot slot)
        {
            if (slot is not EquipmentSlot equipmentSlot) return null;
            return Controller?.Model?.Get(equipmentSlot.EquipmentType);
        }
        public void Bind(EquipmentController controller)
        {
            this.Controller = controller;
        }

        public bool HandleDrop(Slot fromSlot, Slot toSlot, Item item)
        {
            // 장비 슬롯에만 처리
            if (toSlot is not EquipmentSlot targetSlot)
                return false;

            if (item is not EquipmentItem equipItem || equipItem.EquipmentType != targetSlot.EquipmentType)
                return false;

            Controller?.Model?.Add(item);
            return true;
        }
    }
}