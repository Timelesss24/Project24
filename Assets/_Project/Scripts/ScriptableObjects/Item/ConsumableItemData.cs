using UnityEditor;
using UnityEngine;

namespace Timelesss
{
    public enum PotionType
    {
        HP,
        Stamina
    }

    [CreateAssetMenu(fileName = "New Consumable Item Data", menuName = "Item/Consumable Item Data")]
    public class ConsumableItemData : ItemData
    {
        public float effectValue;   // 효과 값 (예: 회복량, 증가량 등)
        public float duration;      // 지속 시간 (0이면 즉시 효과)
        public PotionType type;

        public ItemType itemType = ItemType.UseableItem;

        public override void OnUseItem(PlayerInfo playerInfo)
        {
            switch (type)
            {
                case PotionType.HP:
                    playerInfo.RestoreHealth(effectValue);
                    break;
                case PotionType.Stamina:
                    playerInfo.RestoreStamina(effectValue);
                    break;
            }
        }
    }
}
