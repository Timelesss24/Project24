using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class ConsumableStorage : StorageView, IItemContainer
    {
        [SerializeField] GameObject slotPrefab;
        [SerializeField] Transform slotsParent;
        [SerializeField] Button useItemButton;
        [SerializeField] Image uibCoolTimeIndicator;
        
        [SerializeField] InputReader inputReader;
        
        Item sharedItem;


        void OnEnable()
        {
            inputReader.Consume += Use;
        }

        void OnDisable()
        {
            inputReader.Consume -= Use;
        }
        protected override void Start()
        {
            useItemButton.onClick.AddListener(Use);
        }
       
        void CoolTime(float time, Image indicator)
        {
            indicator.fillAmount = 1f;
            indicator.DOFillAmount(0, time);
        }
        public override void InitializeView(int capacity = 0)
        {
            Slots[0].Initialize(0);

            // 클릭 시 사용 연결
            //slot.OnClick += Use;
        }

        protected override Item GetItemFromSlot(Slot slot) => sharedItem;

        public bool HandleDrop(Slot fromSlot, Slot toSlot, Item item, Action<Item> OnSwap = null)
        {
            if(item == null) return false;
            
            Debug.Log($"[ConsumableStorage] HandleDrop called. Item: {item?.Details?.ItemType}");

            if (item.Details && item.Details.ItemType != ItemType.Consumable)
            {
                Debug.LogWarning("올바르지 않은 아이템입니다.");
                return false;
            }

            sharedItem = item;
            if (item.Details) Slots[0].Set(item.Id, item.Details.Icon, item.Quantity);
            return false;
        }

        void Use()
        {
            if (sharedItem == null || uibCoolTimeIndicator.fillAmount != 0) return;

            CoolTime(5f, uibCoolTimeIndicator);
            
            sharedItem.Quantity--;
            PlayerManager.Instance.PlayerIfo.RestoreHealth(sharedItem.Details.RestoreValue);
            
            
            if (sharedItem.Quantity <= 0)
            {
                PlayerManager.Instance.Inventory.Controller.Model.Remove(sharedItem); // 인벤토리에서 제거
                sharedItem = null;
                Slots[0].Set(SerializableGuid.Empty, null);
            }
            else
            {
                Slots[0].Set(sharedItem.Id, sharedItem.Details.Icon, sharedItem.Quantity);
            }
            
        }
        
    }
}