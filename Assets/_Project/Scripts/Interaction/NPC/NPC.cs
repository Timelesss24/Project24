using UnityEngine;

namespace Timelesss
{
    public class NPC : InteractableBase
    {
        private NPCAnimator animator;

        public override string InteractionName { get; } = "대화하기";

        private void Start()
        {
            animator = GetComponent<NPCAnimator>();
        }

        public override void Interact()
        {
            // 플레이어 조작 불가능 하도록 변경

            // NPC 바라보도록 회전

            // UI 프롬프트 지워주기

            // 대화창 UI 열기

            animator.StartAnimation(animator.talkHash);

            Debug.Log("NPC와 대화");
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);

            if (other.CompareTag(PlayerTag))
                animator.StopAnimation(animator.talkHash);
        }
    }
}

