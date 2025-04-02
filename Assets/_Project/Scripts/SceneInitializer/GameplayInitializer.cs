using _01_Scripts.UI;
using Framework.Audio;
using Managers;
using Scripts.UI;
using Timelesss;
using UnityEngine;

namespace Framework
{
    public class GameplayInitializer : SceneInitializer
    {
        public override void Initialize()
        {
            Debug.Log("Gameplay Initialized");

            // 적 생성
            // EnemyManager.Instance.SpawnEnemies();

            // 게임플레이 UI 표시
            UIManager.Instance.ShowUI<MainUI>();

            if (PlayerManager.Instance.PlayerIfo != null)
                PlayerManager.Instance.PlayerIfo.InitalizedValueChanged();

            // 게임 로직 실행
            Debug.Log("Gameplay is now running.");

            if (GameStateManager.Instance.CurrentState.ToString().Replace("Scene", "") == "Village")
            { SoundManager.Instance.ChangeBGMWithFade("VillageBGM", 1.0f); }
        }
    }
}