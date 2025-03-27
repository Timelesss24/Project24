using UnityEngine;

namespace Timelesss
{
    public class NPC : InteractableBase
    {
        public override string InteractionName { get; } = "대화하기";

        public override void Interact()
        {
            // 플레이어 조작 불가능 하도록 변경

            // NPC 바라보도록 회전

            // UI 프롬프트 지워주기

            // 대화창 UI 열기

            Debug.Log("NPC와 대화");
        }
    }
}

