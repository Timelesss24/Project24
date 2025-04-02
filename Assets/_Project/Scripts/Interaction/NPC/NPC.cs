using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    [System.Serializable]
    public class NPCInfo
    {
        public string Name;
        public int ID;
    }

    public class NPC : InteractableBase
    {
        [SerializeField]
        NPCInfo npcInfo;

        NPCAnimator animator;

        public override string InteractionName { get; } = "대화하기";

        Quaternion startingRotation;
        Transform playerTransform;

        float rotationSpeed = 5f;
        Coroutine rotationCoroutine;

        void Start()
        {
            animator = GetComponent<NPCAnimator>();

            startingRotation = transform.rotation;
            playerTransform = FindObjectOfType<CharacterController>()?.transform;
        }

        public override void Interact()
        {
            base.Interact();
            // 플레이어 조작 불가능 하도록 변경

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

            DialogueManager.Instance.StartDialogue(npcInfo, transform, () =>
                InteractionManager.Instance.EndInteraction());

            // UI 프롬프트 지워주기

            // 대화창 UI 열기

            animator.StartAnimation(animator.talkHash);
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

        IEnumerator RotatingToTarget(object target)
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