using Managers;
using UnityEngine;

namespace Timelesss
{
    public class Portal : InteractableBase
    {
        public override string InteractionName { get; } = "입장하기";

        [SerializeField] private string targetSceneName;

        public override void Interact()
        {
            base.Interact();
            // 입장 확인 팝업 띄워주기
            Debug.Log($"{targetSceneName} 던전 입장");
            
            GameStateManager.Instance.SetGameState(GameStateManager.GameState.Gameplay);
            InteractionManager.Instance.EndInteraction();
        }
    }
}
