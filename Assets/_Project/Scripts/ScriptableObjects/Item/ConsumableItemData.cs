using UnityEditor;
using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "New Consumable Item Data", menuName = "Item/Consumable Item Data")]
    public class ConsumableItemData : ItemData
    {
        public PotionEffect effect;

        public float effectValue;   // 효과 값 (예: 회복량, 증가량 등)
        public float duration;      // 지속 시간 (0이면 즉시 효과)

        public override string ItemType => "소비 아이템";

        public void Use()
        {
            if (effect != null)
            {
                effect.ApplyEffect(effectValue, duration);
            }
            else
            {
                Debug.LogError("포션 효과가 할당되지 않았습니다.");
            }
        }
    }
}
