using System.Collections;
using UnityEngine;

namespace Timelesss
{
    public class NPC : InteractableBase
    {
        private NPCAnimator animator;

        public override string InteractionName { get; } = "대화하기";

        private Quaternion startingRotation;
        private Transform playerTransform;

        private float rotationSpeed = 5f;
        private Coroutine rotationCoroutine;

        private void Start()
        {
            animator = GetComponent<NPCAnimator>();

            startingRotation = transform.rotation;
            playerTransform = FindObjectOfType<CharacterController>()?.transform;
        }

        public override void Interact()
        {
            // 플레이어 조작 불가능 하도록 변경

            // NPC 바라보도록 회전

            if (playerTransform == null)
            {
                playerTransform = FindObjectOfType<CharacterController>()?.transform;

                if(playerTransform == null)
                {
                    Debug.LogWarning("플레이어를 찾을 수 없습니다.");
                    return;
                }
            }


            if (rotationCoroutine != null)
                StopCoroutine(rotationCoroutine);

            rotationCoroutine = StartCoroutine(RotatingToTarget(playerTransform.position));

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

            if (rotationCoroutine != null)
                StopCoroutine(rotationCoroutine);

            rotationCoroutine = StartCoroutine(RotatingToTarget(startingRotation));
        }

        private IEnumerator RotatingToTarget(object target)
        {
            Quaternion targetRotation;

            // 타겟을 바라볼 때
            if (target is Vector3 targetPosition)
            {
                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                directionToTarget.y = 0;
                targetRotation = Quaternion.LookRotation(directionToTarget);
            }
            // 원래의 회전값으로 돌아갈때
            else if (target is Quaternion rotation)
            {
                targetRotation = rotation;
            }
            else
            {
                Debug.LogWarning("RotatingToTarget 코루틴의 매개변수를 다시 설정해주세요.");
                yield break;
            }

            while (true)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    transform.rotation = targetRotation;
                    yield break;
                }

                yield return null;
            }
        }
    }
}