using Managers;
using UnityEngine;


namespace Timelesss
{
    public class ToVillagePotal : InteractableBase
    {
        public override string InteractionName { get; } = "돌아가기";

        [SerializeField] private string targetSceneName;

        public override void Interact()
        {
            base.Interact();
            // 입장 확인 팝업 띄워주기
            Debug.Log($"{targetSceneName} 마을 돌아가기 입장");


            var popup = UIManager.Instance.ShowPopup<ConfirmPopup>();
            popup.InitailizePoup("마을로 돌아가시겠습니까?", () => {
                InteractionManager.Instance.EndInteraction();
                GameStateManager.Instance.SetGameState(GameStateManager.GameState.Village);
            }, () => InteractionManager.Instance.EndInteraction());

        }
    }
}
