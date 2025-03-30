using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public abstract class PotionEffect : ScriptableObject
    {
        public abstract void ApplyEffect(float effectValue, float duration);
    }

    [CreateAssetMenu(fileName = "Health Potion Effect", menuName = "Item/Potion Effect/Health Potion Effect")]
    public class HealthPotionEffect : PotionEffect
    {
        public override void ApplyEffect(float effectValue, float duration)
        {
            // 체력 회복 함수 실행
            Debug.Log($"체력 회복: {effectValue}");
        }
    }

    [CreateAssetMenu(fileName = "Stamina Potion Effect", menuName = "Item/Potion Effect/Stamina Potion Effect")]
    public class StaminaPotionEffect : PotionEffect
    {
        public override void ApplyEffect(float effectValue, float duration)
        {
            // 스태미너 회복 함수 실행
            Debug.Log($"스태미너 회복: {effectValue}");
        }
    }
}
