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
            Debug.Log($"{damage}�� �������� �Ծ����ϴ�. ���� ü�� : {currentHealth}/{maxHealth}");
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
                    Debug.LogWarning("�� �� ���� ���� Ÿ���Դϴ�.");
                    break;
            }
        }

        private void RestoreHealth(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + Mathf.RoundToInt(value), 0, maxHealth);
            Debug.Log($"ü���� {value}��ŭ ȸ���Ǿ����ϴ�. ���� ü��: {currentHealth}/{maxHealth}");
        }

        private void RestoreStamina(float value)
        {
            currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);
            Debug.Log($"���¹̳ʰ� {value}��ŭ ȸ���Ǿ����ϴ�. ���� ���¹̳�: {currentStamina}/{maxStamina}");
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
                EditorGUILayout.LabelField("ü�� ����", EditorStyles.boldLabel);

                healAmount = EditorGUILayout.IntSlider("ȸ����", healAmount, 20, 150);
                if (GUILayout.Button("ü�� ȸ��"))
                {
                    playerInfo.UsePotion(PotionType.HP, healAmount);
                }

                EditorGUILayout.Space();

                damageAmount = EditorGUILayout.IntSlider("������", damageAmount, 1, 100);
                if (GUILayout.Button("������ ������"))
                {
                    playerInfo.TakeDamage(damageAmount);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("�÷��� ��忡���� ����� �� �ֽ��ϴ�.", MessageType.Warning);
            }
        }
    }
}
