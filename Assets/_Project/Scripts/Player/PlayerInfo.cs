﻿using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using Systems.Persistence;
using UnityEditor;
using UnityEngine;

namespace Timelesss
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }

        public int PlayerLevel;
        public int MaxHealth;
        public int CurrentHealth;
        public int CurrentExp;
        public int RequiredExp;
    }

    public class PlayerInfo : ValidatedMonoBehaviour, IDamageable, IBind<PlayerData>
    {
        [SerializeField, Self] PlayerController playerController;

        public string PlayerName { get; private set; }

        private int playerLevel = 1;

        private int totalMaxHealth { get => baseMaxHealth + equipmentMaxHealth; }
        private int baseMaxHealth = 100;
        private int equipmentMaxHealth = 0;
        private int currentHealth = 100;

        private float maxStamina = 100;
        private float currentStamina = 100;

        public int totalDeffence { get => baseDeffence + equipmentDeffecne; }
        private int baseDeffence = 5;
        private int equipmentDeffecne = 0;

        public int totalAttack { get => baseAttack + equipmentAttack; }
        private int baseAttack = 100;
        private int equipmentAttack = 0;

        private int currentExp = 0;
        private int requiredExp = 100;

        [SerializeField] private EventChannel<float> hpChangedEvent;
        [SerializeField] private EventChannel<float> staminaChangedEvent;
        [SerializeField] private EventChannel<float> expChangedEvent;
        [SerializeField] private EventChannel<int> levelChangedEvent;

        public event Action ExhanstedAction;
        public event Action DeathAction;

        private IEnumerator staminaCoroutine;

        [SerializeField] PlayerData playerData;
        private event Action DataChangedAction;


        private void Start()
        {
            GetName();
            LoadPlayerData();

            DataChangedAction += SavePlayerData;
        }

        public void InitalizedValueChanged()
        {
            hpChangedEvent?.Invoke((float)currentHealth/ (float)totalMaxHealth);
            staminaChangedEvent?.Invoke((float)currentStamina/ (float)maxStamina);
            expChangedEvent?.Invoke((float)currentExp/ (float)requiredExp);
            levelChangedEvent?.Invoke(playerLevel);
        }

        public string GetName()
        {
            if (PlayerName != null) return PlayerName;

            PlayerName = PlayerPrefs.GetString("PlayerName", string.Empty);

            if (PlayerName == string.Empty)
                PlayerName = "유니티24조";

            return PlayerName;
        }

        public void ApplyEquipStatus(EquipItemData itemdata)
        {
            switch (itemdata.EquipType)
            {
                case EquipType.Null:
                    break;
                case EquipType.Sword:
                    equipmentAttack = (int)itemdata.EquipValue;
                    break;
                case EquipType.Helmet:
                    equipmentMaxHealth = (int)itemdata.EquipValue;
                    break;
                default:
                    break;
            }
        }

        public void RemoveEquipStatus(EquipItemData itemdata)
        {
            switch (itemdata.EquipType)
            {
                case EquipType.Null:
                    break;
                case EquipType.Sword:
                    equipmentAttack = 0;
                    break;
                case EquipType.Helmet:
                    equipmentMaxHealth = 0;
                    break;
                default:
                    break;
            }
        }

        public event Action OnDamageTaken;
        public void TakeDamage(int value)
        {
            if (!playerController.CanHit) return;

            float damageReductionFactor = (float)value / (value + totalDeffence);
            int reducedDamage = Mathf.RoundToInt(value * damageReductionFactor);

            currentHealth = Mathf.Clamp(currentHealth - reducedDamage, 0, totalMaxHealth);
            hpChangedEvent?.Invoke((float)currentHealth / (float)totalMaxHealth);
            if (reducedDamage > 0) OnDamageTaken?.Invoke();
            if (currentHealth <= 0f)
                DeathAction?.Invoke();

            SaveLoadSystem.Instance.GameData.PlayerData.CurrentHealth = currentHealth;
            SaveLoadSystem.Instance.GameData.PlayerData.MaxHealth = baseMaxHealth;
        }

        public bool UseStamina(float value)
        {
            if (currentStamina <= 0f)
            {
                ExhanstedAction?.Invoke();
                return false;
            }

            if (staminaCoroutine != null)
            {
                StopCoroutine(staminaCoroutine);
                staminaCoroutine = null;
            }

            staminaCoroutine = RestoreStaminaCoroutine();
            StartCoroutine(staminaCoroutine);

            currentStamina = Mathf.Clamp(currentStamina - value, 0, maxStamina);
            staminaChangedEvent?.Invoke((float)currentStamina / (float)maxStamina);
            return true;
        }

        public void RestoreHealth(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + Mathf.RoundToInt(value), 0, totalMaxHealth);
            hpChangedEvent?.Invoke((float)currentHealth/ (float)totalMaxHealth);
            if (currentHealth <= 0f)
                DeathAction?.Invoke();

            SaveLoadSystem.Instance.GameData.PlayerData.CurrentHealth = currentHealth;
            SaveLoadSystem.Instance.GameData.PlayerData.MaxHealth = baseMaxHealth;
            Debug.Log($"체력이 {value}만큼 회복되었습니다. 현재 체력: {currentHealth}/{totalMaxHealth}");
        }

        public void RestoreStamina(float value)
        {
            currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);
            staminaChangedEvent?.Invoke((float)currentStamina / (float)maxStamina);

            if (value > 1)
                Debug.Log($"스태미너가 {value}만큼 회복되었습니다. 현재 스태미너: {currentStamina}/{maxStamina}");
        }

        private IEnumerator RestoreStaminaCoroutine()
        {
            yield return new WaitForSeconds(1f);

            float restoreValue = 0.2f;
            float waitTime = 0.03f;

            while (currentHealth <= maxStamina)
            {
                RestoreStamina(restoreValue);
                yield return new WaitForSeconds(waitTime);
            }

            staminaCoroutine = null;
        }

        public void IncreasedExp(int value)
        {
            currentExp += value;
            Debug.Log($"{value} 경험치를 획득하였습니다. 현재 경험치: {currentExp}/{requiredExp}");

            while (currentExp >= requiredExp && playerLevel < 100)
            {
                currentExp -= requiredExp;
                LevelUp();
            }
            SaveLoadSystem.Instance.GameData.PlayerData.CurrentExp = currentExp;
            expChangedEvent?.Invoke((float)currentExp / (float)requiredExp);
        }

        private void LevelUp()
        {
            baseMaxHealth = (int)((float)baseMaxHealth * 1.15f);
            maxStamina += 10f;

            baseAttack += Mathf.Max((int)(baseAttack * 0.15f), 1);
            baseDeffence += Mathf.Max((int)(baseDeffence * 0.15f), 1);

            playerLevel++;
            requiredExp = (int)((float)requiredExp * 1.25f);

            levelChangedEvent?.Invoke(playerLevel);

            SaveLoadSystem.Instance.GameData.PlayerData.PlayerLevel = playerLevel;
            SaveLoadSystem.Instance.GameData.PlayerData.CurrentExp = currentExp;
            SaveLoadSystem.Instance.GameData.PlayerData.RequiredExp = requiredExp;

            Debug.Log($"레벨업. 현재 레벨: {playerLevel} / 현재 경험치: {currentExp}/{requiredExp}");
        }

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
        public void Bind(PlayerData data)
        {
            playerData = data;
            Id = data.Id;
            playerLevel = data.PlayerLevel;
            baseMaxHealth = data.MaxHealth;
            currentHealth = data.CurrentHealth;
            currentExp = data.CurrentExp;
            requiredExp = data.RequiredExp;
        }

        private void SavePlayerData()
        {
            SaveLoadSystem.Instance.GameData.PlayerData = playerData;
            SaveLoadSystem.Instance.SaveGame();
        }

        private void LoadPlayerData()
        {
            playerData = SaveLoadSystem.Instance.LoadGame(SaveLoadSystem.Instance.GameData.Name).PlayerData;

            if (playerData != null)
                Bind(playerData);
        }
    }

    [CustomEditor(typeof(PlayerInfo))]
    public class PlayerInfoEditor : Editor
    {
        private int healAmount = 10;
        private int damageAmount = 10;
        private int expAmount;

        private float staminaRecoverAmount = 10f;
        private float staminaUseAmount = 10f;

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
                    playerInfo.RestoreHealth(healAmount);
                }

                EditorGUILayout.Space();

                damageAmount = EditorGUILayout.IntSlider("데미지", damageAmount, 1, 100);
                if (GUILayout.Button("데미지 입히기"))
                {
                    playerInfo.TakeDamage(damageAmount);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("스태미너 조정", EditorStyles.boldLabel);

                staminaRecoverAmount = EditorGUILayout.Slider("스태미너 회복량", staminaRecoverAmount, 1f, 100f);
                if (GUILayout.Button("스태미너 회복"))
                {
                    playerInfo.RestoreStamina(staminaRecoverAmount);
                }

                staminaUseAmount = EditorGUILayout.Slider("스태미너 사용량", staminaUseAmount, 1f, 100f);
                if (GUILayout.Button("스태미너 사용"))
                {
                    playerInfo.UseStamina(staminaUseAmount);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("경험치 획득", EditorStyles.boldLabel);

                expAmount = EditorGUILayout.IntSlider("경험치 획득량", expAmount, 50, 150);
                if (GUILayout.Button("경험치 증가"))
                {
                    playerInfo.IncreasedExp(expAmount);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("플레이 모드에서만 사용할 수 있습니다.", MessageType.Warning);
            }
        }
    }
}
