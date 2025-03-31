using UnityEngine;

namespace Timelesss
{
    public enum EquipType
    {
        Sword
    }

    [CreateAssetMenu(fileName = "New Equip Item Data", menuName = "Item/Equip Item Data")]
    public class EquipItemData : ItemData
    {
        public EquipType equipType;

        public override void OnUseItem()
        {

        }
    }
}
