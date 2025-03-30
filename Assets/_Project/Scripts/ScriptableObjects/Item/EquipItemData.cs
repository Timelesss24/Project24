using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "New Equip Item Data", menuName = "Item/Equip Item Data")]
    public class EquipItemData : ItemData
    {
        public override string ItemType => "장비 아이템";
    }
}
