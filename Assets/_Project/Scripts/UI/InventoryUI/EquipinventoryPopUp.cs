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
        [SerializeField] Image helmetEquipIcon;
        [SerializeField] Image armorEquipIcon;
        [SerializeField] Image bootsEquipIcon;

        public void OnEquipItem(EquipItemData itemData)
        {
            switch (itemData.EquipType)
            {
                case EquipType.Null:
                    break;
                case EquipType.Sword:
                    swordEquipIcon.sprite = itemData.ItemIcon;
                    break;
                case EquipType.Helmet:
                    helmetEquipIcon.sprite = itemData.ItemIcon;
                    break;
                default:
                    break;
            }
        }
    }
}
