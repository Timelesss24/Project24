using System;
using System.Collections;
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

        int TotalMaxHealth => playerData?.MaxHealth ?? 100;//{ get => baseMaxHealth + equipmentMaxHealth; }
        //private int equipmentMaxHealth = 0;

        float maxStamina = 100;
        float currentStamina = 100;

        public int TotalDeffence => baseDeffence + equipmentDeffecne;
        int baseDeffence = 5;
        int equipmentDeffecne = 0;

        public int TotalAttack => baseAttack + equipmentAttack;
        int baseAttack = 100;
        int equipmentAttack = 0;

        [SerializeField]
        EventChannel<float> hpChangedEvent;
        [SerializeField]
        EventChannel<float> staminaChangedEvent;
        [SerializeField]
        EventChannel<float> expChangedEvent;
        [SerializeField]
        EventChannel<int> levelChangedEvent;

        public event Action ExhanstedAction;
        public event Action DeathAction;

        IEnumerator staminaCoroutine;
        
        [SerializeField] PlayerData playerData;
        event Action DataChangedAction;


        void Start()
        {
            GetName();
            //LoadPlayerData();

            //DataChangedAction += SavePlayerData;
        }

        public void InitalizedValueChanged()
        {
            hpChangedEvent?.Invoke(playerData.CurrentHealth);
            staminaChangedEvent?.Invoke(currentStamina);
            expChangedEvent?.Invoke(playerData.CurrentExp);
            levelChangedEvent?.Invoke(playerData.PlayerLevel);
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
                    //equipmentMaxHealth = (int)itemdata.EquipValue;
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
                   // equipmentMaxHealth = 0;
                    break;
                default:
                    break;
            }
        }

        public event Action OnDamageTaken;
        public void TakeDamage(int value)
        {
            if(!playerController.CanHit) return;
            
            float damageReductionFactor = (float)value / (value + TotalDeffence);
            int reducedDamage = Mathf.RoundToInt(value * damageReductionFactor);

            playerData.CurrentHealth = Mathf.Clamp(playerData.CurrentHealth - reducedDamage, 0, TotalMaxHealth);
            hpChangedEvent?.Invoke(playerData.CurrentHealth);
            if (reducedDamage > 0) OnDamageTaken?.Invoke();
            if(playerData.CurrentHealth <= 0f)
                DeathAction?.Invoke();
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
            staminaChangedEvent?.Invoke(currentStamina);
            return true;
        }

        public void RestoreHealth(float value)
        {
            playerData.CurrentHealth = Mathf.Clamp(playerData.CurrentHealth + Mathf.RoundToInt(value), 0, TotalMaxHealth);
            hpChangedEvent?.Invoke(playerData.CurrentHealth);
            if(playerData.CurrentHealth <= 0f)
                DeathAction?.Invoke();
            
            Debug.Log($"체력이 {value}만큼 회복되었습니다. 현재 체력: {playerData.CurrentHealth}/{TotalMaxHealth}");
        }

        public void RestoreStamina(float value)
        {
            currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);
            staminaChangedEvent?.Invoke(currentStamina);

            if (value > 1)
                Debug.Log($"스태미너가 {value}만큼 회복되었습니다. 현재 스태미너: {currentStamina}/{maxStamina}");
        }

        IEnumerator RestoreStaminaCoroutine()
        {
            yield return new WaitForSeconds(1f);

            float restoreValue = 0.2f;
            float waitTime = 0.03f;

            while (playerData.CurrentHealth <= maxStamina)
            {
                RestoreStamina(restoreValue);
                yield return new WaitForSeconds(waitTime);
            }

            staminaCoroutine = null;
        }

        public void IncreasedExp(int value)
        {
            playerData.CurrentExp += value;            
            Debug.Log($"{value} 경험치를 획득하였습니다. 현재 경험치: {playerData.CurrentExp}/{playerData.RequiredExp}");
            
            while (playerData.CurrentExp >= playerData.RequiredExp && playerData.PlayerLevel < 100)
            {
                playerData.CurrentExp -= playerData.RequiredExp;
                LevelUp();
            }
            expChangedEvent?.Invoke(playerData.CurrentExp);
        }

        void LevelUp()
        {
            playerData.MaxHealth = (int)((float)playerData.MaxHealth * 1.15f);
            maxStamina += 10f;

            baseAttack += Mathf.Max((int)(baseAttack * 0.15f), 1);
            baseDeffence += Mathf.Max((int)(baseDeffence * 0.15f), 1);

            playerData.PlayerLevel++;
            playerData.RequiredExp = (int)((float)playerData.RequiredExp * 1.25f);

            levelChangedEvent?.Invoke(playerData.PlayerLevel);
            
            
            Debug.Log($"레벨업. 현재 레벨: {playerData.PlayerLevel} / 현재 경험치: {playerData.CurrentExp}/{playerData.RequiredExp}");
        }
        
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
        
        public void Bind(PlayerData data)
        {
            playerData = data;
            playerData.Id = Id;
            
            //Debug.Log("Bind PlayerData:" + data.Id.ToGuid());
            // playerData.PlayerLevel = data.PlayerLevel;
            // playerData.MaxHealth = data.MaxHealth;
            // playerData.CurrentHealth = data.CurrentHealth;
            // playerData.CurrentExp = data.CurrentExp;
            //playerData.requiredExp = data.RequiredExp;
        }

        // void SavePlayerData()
        // {
        //     SaveLoadSystem.Instance.GameData.PlayerData = playerData;
        //     SaveLoadSystem.Instance.SaveGame();
        // }

        // void LoadPlayerData()
        // {
        //     playerData = SaveLoadSystem.Instance.LoadGame(SaveLoadSystem.Instance.GameData.Name).PlayerData;
        //     
        //     if (playerData != null)
        //         Bind(playerData);
        // }
    }

    [CustomEditor(typeof(PlayerInfo))]
    public class PlayerInfoEditor : Editor
    {
        int healAmount = 10;
        int damageAmount = 10;
        int expAmount;

        float staminaRecoverAmount = 10f;
        float staminaUseAmount = 10f;

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
