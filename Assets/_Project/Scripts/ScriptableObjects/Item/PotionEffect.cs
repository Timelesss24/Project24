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
            // ü�� ȸ�� �Լ� ����
            Debug.Log($"ü�� ȸ��: {effectValue}");
        }
    }

    [CreateAssetMenu(fileName = "Stamina Potion Effect", menuName = "Item/Potion Effect/Stamina Potion Effect")]
    public class StaminaPotionEffect : PotionEffect
    {
        public override void ApplyEffect(float effectValue, float duration)
        {
            // ���¹̳� ȸ�� �Լ� ����
            Debug.Log($"���¹̳� ȸ��: {effectValue}");
        }
    }
}
