using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityUtils;

namespace Systems.Persistence {
    [Serializable] public class GameData { 
        public string Name;
       // public string CurrentLevelName;
    }
        
    public interface ISaveable  {
        SerializableGuid Id { get; set; }
    }
    
    public interface IBind<TData> where TData : ISaveable {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
    }
    
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem> {
        [FormerlySerializedAs("gameData")]
        [SerializeField] public GameData GameData;

        IDataService dataService;

        protected override void Awake() {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }
        
        void Start() => NewGame();

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            
            //Bind<T,TData>(data)
        }
        
        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T: MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach(var entity in entities) {
                var data = datas.FirstOrDefault(d=> d.Id == entity.Id);
                if (data == null) {
                    data = new TData { Id = entity.Id };
                    datas.Add(data); 
                }
                entity.Bind(data);
            }
        }

        public void NewGame() {
            GameData = new GameData {
                Name = "Game",
                //CurrentLevelName = "Demo"
            };
            //SceneManager.LoadScene(GameData.CurrentLevelName);
        }
        
        public void SaveGame() => dataService.Save(GameData);

        public void LoadGame(string gameName) {
            GameData = dataService.Load(gameName);

            // if (String.IsNullOrWhiteSpace(GameData.CurrentLevelName)) {
            //     GameData.CurrentLevelName = "Demo";
            // }
            //
            // SceneManager.LoadScene(GameData.CurrentLevelName);
        }
        
        public void ReloadGame() => LoadGame(GameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}