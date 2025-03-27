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
            // 입장 확인 팝업 띄워주기

            // ConfirmPopup.SetUp("던전으로 입장하시겠습니까?, () => SceneLoader.Instance.LoadScene(targetSceneName)");

            Debug.Log($"{targetSceneName} 던전 입장");
            SceneLoader.Instance.LoadScene(targetSceneName); // 테스트용
        }
    }
}
