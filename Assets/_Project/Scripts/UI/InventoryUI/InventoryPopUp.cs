using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class InventoryPopUp : UIPopup
    {
        [SerializeField] Button closeButton;
        [SerializeField] GameObject itemSlot;
        [SerializeField] Transform itemSlotParent;

        PlayerInventory playerInventory;

        private void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);
        }

        private void Start()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            CreateInventoryItems();
        }

        void OnClickCloseButton()
        {
            ClosePopup();
        }

        void CreateInventoryItems()
        {
            foreach (ItemData itemData in playerInventory.playerItems)
            {
                CreateUIItem(itemData);
            }
        }
        
        void CreateUIItem(ItemData itemData)
        {
            ItemSlot go = Instantiate(itemSlot, itemSlotParent).GetComponent<ItemSlot>();
            go.itemIcon.sprite = itemData.ItemIcon;
            go.itemData = itemData;
        }
    }
}
