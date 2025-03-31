using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class EquipinventoryPopUp : UIPopup
    {
        [SerializeField] Image swordEquipIcon;

        public void OnEquipItem(ItemData itemData)
        {
            swordEquipIcon.GetComponent<Image>().sprite = itemData.ItemIcon;
        }
    }
}
