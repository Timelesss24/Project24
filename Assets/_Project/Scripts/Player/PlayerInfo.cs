using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Timelesss
{
    public class PlayerInfo : MonoBehaviour
    {
        public int currentHealth = 100;
        public int maxHealth = 100;

        public float currentStamina = 100;
        public float maxStamina = 100;

        public void TakeDamage(int damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            Debug.Log($"{damage}의 데미지를 입었습니다. 현재 체력 : {currentHealth}/{maxHealth}");
        }

        public void UsePotion(PotionType potionType, float effectValue, float duration = 0)
        {
            switch (potionType)
            {
                case PotionType.HP:
                    RestoreHealth(effectValue);
                    break;

                case PotionType.Stamina:
                    RestoreStamina(effectValue);
                    break;

                default:
                    Debug.LogWarning("알 수 없는 포션 타입입니다.");
                    break;
            }
        }

        private void RestoreHealth(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + Mathf.RoundToInt(value), 0, maxHealth);
            Debug.Log($"체력이 {value}만큼 회복되었습니다. 현재 체력: {currentHealth}/{maxHealth}");
        }

        private void RestoreStamina(float value)
        {
            currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);
            Debug.Log($"스태미너가 {value}만큼 회복되었습니다. 현재 스태미너: {currentStamina}/{maxStamina}");
        }
    }

    [CustomEditor(typeof(PlayerInfo))]
    public class PlayerInfoEditor : Editor
    {
        private int healAmount = 10;
        private int damageAmount = 10;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PlayerInfo playerInfo = (PlayerInfo)target;

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("체력 조정", EditorStyles.boldLabel);

                healAmount = EditorGUILayout.IntSlider("회복량", healAmount, 20, 150);
                if (GUILayout.Button("체력 회복"))
                {
                    playerInfo.UsePotion(PotionType.HP, healAmount);
                }

                EditorGUILayout.Space();

                damageAmount = EditorGUILayout.IntSlider("데미지", damageAmount, 1, 100);
                if (GUILayout.Button("데미지 입히기"))
                {
                    playerInfo.TakeDamage(damageAmount);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("플레이 모드에서만 사용할 수 있습니다.", MessageType.Warning);
            }
        }
    }
}
