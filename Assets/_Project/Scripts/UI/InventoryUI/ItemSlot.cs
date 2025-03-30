using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Timelesss
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ItemData itemData;
        [SerializeField] GameObject itemDescriptionObject;
        [SerializeField] private Image itemIcon;

        private void Start()
        {
            itemIcon.sprite = itemData.ItemIcon;
            itemDescriptionObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemData.ItemName;
            itemDescriptionObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemData.ItemDescription;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            itemDescriptionObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            itemDescriptionObject.SetActive(false);
        }
    }
}
