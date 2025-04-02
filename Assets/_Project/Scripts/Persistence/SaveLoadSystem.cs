using System;
using System.Collections.Generic;
using System.Linq;
using Timelesss;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityUtils;

namespace Systems.Persistence {
    [Serializable] public class GameData 
    { 
        public string Name = "Game";
        public PlayerData PlayerData;
        public SaveableQuestData QuestData;
        public InventoryData InventoryData;
    }
        
    public interface ISaveable  
    {
        SerializableGuid Id { get; set; }
    }
    
    public interface IBind<TData> where TData : ISaveable 
    {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
    }
    
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem> 
    {
        [FormerlySerializedAs("gameData")]
        [SerializeField] public GameData GameData;

        IDataService dataService;

        protected override void Awake() 
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }
        
        void Start() => NewGame();

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
        {
            //SaveGame();
            LoadGame(GameData.Name);
            AllBind();
        }

        public void AllBind()
        {
            Bind<PlayerInfo, PlayerData>(GameData.PlayerData);
            Bind<QuestManager, SaveableQuestData>(GameData.QuestData);
            Bind<Timelesss.Inventory, InventoryData>(GameData.InventoryData);
        }
        
        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() 
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                Debug.Log($"Bind {entity.name}");
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T: MonoBehaviour, IBind<TData> where TData : ISaveable, new() 
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach(var entity in entities) 
            {
                var data = datas.FirstOrDefault(d=> d.Id == entity.Id);
                if (data == null) {
                    data = new TData { Id = entity.Id };
                    datas.Add(data); 
                }
                entity.Bind(data);
            }
        }

        public void NewGame() 
        {
            GameData = new GameData 
            {
                Name = "Game",
                PlayerData = new PlayerData 
                {
                    PlayerLevel = 1,
                    CurrentHealth = 80,
                    MaxHealth = 100,
                    CurrentExp = 0,
                    RequiredExp = 100
                },
                QuestData = new SaveableQuestData 
                {
                    ActiveQuestList = new List<ActiveQuestInfo>(),
                    CompleteQuestList = new List<int>(),
                },
                InventoryData = new InventoryData(),
     
            };
        }

        public void SaveGame()
        {
            Debug.Log("세이브세이브세이브");
            dataService.Save(GameData);   
        }

        public void LoadGame(string gameName)
        {
            GameData = dataService.Load(gameName);
            
            
            //return dataService.Load(gameName);
        }
        
        public void ReloadGame() => LoadGame(GameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);

        void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}