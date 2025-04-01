using Managers;
using UnityEngine;

namespace Timelesss
{
    public class Portal : InteractableBase
    {
        public override string InteractionName { get; } = "입장하기";

        public override void Interact()
        {
            base.Interact();
            // 입장 확인 팝업 띄워주기
            

            var popup = UIManager.Instance.ShowPopup<ConfirmPopup>();
            popup.InitailizePoup("던전에 입장 하시겠습니깐?", () => {
                InteractionManager.Instance.EndInteraction();
                GameStateManager.Instance.SetGameState(GameStateManager.GameState.Gameplay);
            }, () => InteractionManager.Instance.EndInteraction());

        }
    }
}
