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

        private void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);
        }

        void OnClickCloseButton()
        {
            ClosePopup();
        }

        void CreateItems()
        {
            Instantiate(itemSlot, itemSlotParent);
        }

        public void DropItemAddToInventory(ItemData itemData)
        {
            Debug.Log($"{itemData}");
        }
    }
}
