using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "New Equip Item Data", menuName = "Item/Equip Item Data")]
    public class EquipItemData : ItemData
    {
        public ItemType itemType = ItemType.EquipableItem;

        public Material Material;

        public override void OnUseItem(PlayerInfo playerInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
