using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Timelesss
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public ItemData itemData;
        [SerializeField] GameObject itemDescriptionObject;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Button useItemButton;
        [SerializeField] private PlayerInfo playerInfo;
        public Action<PlayerInfo> UseItemAction;

        private void Start()
        {
            itemIcon.sprite = itemData.ItemIcon;
            itemDescriptionObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemData.ItemName;
            itemDescriptionObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemData.ItemDescription;
            useItemButton.onClick.AddListener(OnClickUseItemButton);
            UseItemAction += itemData.OnUseItem;
            playerInfo = FindObjectOfType<PlayerInfo>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            itemDescriptionObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            itemDescriptionObject.SetActive(false);
            useItemButton.gameObject.SetActive(false);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right) // 우클릭 감지
            {
                useItemButton.gameObject.SetActive(true);
                itemDescriptionObject.SetActive(false);
            }
        }

        public void OnClickUseItemButton()
        {
            UseItemAction?.Invoke(playerInfo);
            Destroy(gameObject);
        }

    }
}
