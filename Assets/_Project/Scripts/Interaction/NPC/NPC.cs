using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class NPC : InteractableBase
    {
        [SerializeField] private int npcID;

        private NPCAnimator animator;

        public override string InteractionName { get; } = "대화하기";

        private Quaternion startingRotation;
        private Transform playerTransform;

        private float rotationSpeed = 5f;
        private Coroutine rotationCoroutine;

        private TestDialogueDataLoader test;
        private int currentDialogueID;

        private void Start()
        {
            animator = GetComponent<NPCAnimator>();

            test = new TestDialogueDataLoader();

            startingRotation = transform.rotation;
            playerTransform = FindObjectOfType<CharacterController>()?.transform;

            currentDialogueID = FindFirstDialogueID();
        }

        public override void Interact()
        {
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

            // UI 프롬프트 지워주기

            // 대화창 UI 열기

            animator.StartAnimation(animator.talkHash);

            ShowCurrentDialogue();
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

        private int FindFirstDialogueID()
        {
            foreach (var data in test.ItemsDict.Values)
            {
                if (data.npcID == npcID) 
                {
                    return data.key;
                }
            }

            Debug.LogWarning($"NPC {npcID}의 첫 번째 대화를 찾을 수 없습니다.");
            return -1;
        }


        private void ShowCurrentDialogue()
        {
            if (!test.ItemsDict.TryGetValue(currentDialogueID, out var dialogue))
            {
                Debug.LogWarning($"현재 대화 ID {currentDialogueID}에 해당하는 데이터를 찾을 수 없습니다.");
                return;
            }

            Debug.Log($"NPC {npcID}: {dialogue.dialogueText}");

            if (dialogue.nextDialogueID != 0) 
            {
                currentDialogueID = dialogue.nextDialogueID; 
                Debug.Log($"다음 대화 ID: {currentDialogueID}");
            }
            else
            {
                Debug.Log("대화 종료");
                currentDialogueID = FindFirstDialogueID();
            }
        }
    }
}