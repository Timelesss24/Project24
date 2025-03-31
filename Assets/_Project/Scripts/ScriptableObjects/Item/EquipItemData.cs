using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "New Equip Item Data", menuName = "Item/Equip Item Data")]
    public class EquipItemData : ItemData
    {
        public Material Material;

        public override void OnUseItem()
        {
            PlayerManager.Instance.Player.GetComponent<PlayerInfo>().ApplyEquipStatus(this);
            PlayerManager.Instance.Player.GetComponent<PlayerEquip>().ChangeMaterial(this);
        }
    }
}
