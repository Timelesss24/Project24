using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace Managers
{
    /// <summary>
    /// 게임 상태 및 전체 흐름을 관리하는 기본 GameManager 클래스.
    /// </summary>
    public class GameStateManager : PersistentSingleton<GameStateManager>
    {
        public enum GameState
        {
            TitleScene,
            Gameplay,
        }

        [Header("Game State")] [SerializeField]
        private GameState currentState = GameState.Gameplay;

        /// <summary>
        /// 현재 게임 상태를 제공.
        /// </summary>
        public GameState CurrentState => currentState;
        
        void OnEnable() => SceneManager.sceneLoaded += InitializeScene;
        void OnDisable() => SceneManager.sceneLoaded -= InitializeScene;
        
        
        /// <summary>
        /// 게임 상태를 변경합니다.
        /// </summary>
        /// <param name="newState">새로운 게임 상태</param>
        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            LoadSceneByGameState(currentState);
        }

        void LoadSceneByGameState(GameState state)
        {
            string sceneName = GetSceneNameByGameState(state);

            // 씬 로드
            Debug.Log($"Loading Scene: {sceneName}");
            SceneLoader.Instance.LoadScene(sceneName);
        }
        
        /// <summary>
        /// 씬 초기화
        /// </summary>
        void InitializeScene(Scene scene, LoadSceneMode mode)
        {
            var initializer = FindObjectOfType<SceneInitializer>();
            if (initializer)
            {
                initializer.Initialize();
            }
            else
            {
                Debug.LogWarning("No SceneInitializer found in the current scene.");
            }
        }

        private string GetSceneNameByGameState(GameState state)
        {
            return state switch
            {
                GameState.TitleScene => "TitleScene",
                GameState.Gameplay => "Gameplay",
                _ => string.Empty,
            };
        }
        

        /// <summary>
        /// 게임 종료 처리 (필요 시 확장 가능).
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }
}