using System;
using Managers;
using System.Collections;
using UnityEngine;

namespace Timelesss
{
    public class DialogueManager : UnityUtils.Singleton<DialogueManager>
    {
        DialogueDataLoader dataLoader;
        public int currentDialogueID; // 현재 대화 ID
        int npcID; // 현재 대화 중인 NPC의 ID

        DialoguePopUp dialoguePopUp; // 대화 팝업 UI

        Camera npcCamera; // NPC 카메라

        const int EndOfDialogueID = 0; // 대화의 끝을 나타내는 ID
        const int NoDataID = -1; // 데이터를 찾을 수 없음을 나타내는 ID

        void Start()
        {
            dataLoader = new DialogueDataLoader(); // 대화 데이터 로드

            npcCamera = GetComponentInChildren<Camera>(); 
        }

        public void StartDialogue(NPCInfo npcInfo, Transform npcTransform, Action onComplete = null)
        {
            this.npcID = npcInfo.ID; // NPC ID 설정

            int questID = QuestManager.Instance.GetQuestID(npcInfo.ID); // NPC의 퀘스트 ID 가져오기

            bool hasCompleted = QuestManager.Instance.GetIsCompleteToNpc(npcInfo.ID); // NPC의 퀘스트 완료 여부 확인

            currentDialogueID = hasCompleted ?
                FindCompletedDialogueID(npcInfo.ID) : FindFirstDialogueID(npcInfo.ID, questID != 0); // 대화 ID 설정

            if (currentDialogueID == NoDataID)
            {
                onComplete?.Invoke(); // 대화 데이터가 없으면 완료 콜백 호출
                Debug.LogWarning($"NPC {npcInfo.ID}의 대화 데이터를 찾을 수 없습니다.");
                return;
            }

            if (npcCamera != null)
            {
                npcCamera.transform.parent = npcTransform; // NPC 카메라의 부모를 NPC로 설정
                npcCamera.transform.localPosition = new Vector3(0, 1.7f, 1f); // 카메라 위치 설정
                npcCamera.transform.localRotation = Quaternion.Euler(0, 180f, 0); // 카메라 회전 설정
            }

            if (dialoguePopUp == null)
            {
                dialoguePopUp = UIManager.Instance.ShowPopup<DialoguePopUp>(); // 대화 팝업 UI 표시
                dialoguePopUp.SetNpcID(npcInfo.ID); // 대화 팝업에 NPC ID 설정
            }

            dialoguePopUp.Show(); // 대화 팝업 표시
            dialoguePopUp.SetNpcNameText(npcInfo.Name); // 대화 팝업에 NPC 이름 설정

            ShowCurrentDialogue(); // 현재 대화 표시

            StartCoroutine(TrackingDialogue(onComplete)); // 대화 추적 코루틴 시작
        }

        IEnumerator TrackingDialogue(Action onComplete = null)
        {
            yield return new WaitWhile(() => dialoguePopUp != null); // 대화 팝업이 닫힐 때까지 대기

            onComplete?.Invoke(); // 완료 콜백 호출
        }

        public void ShowCurrentDialogue()
        {
            if (!dataLoader.ItemsDict.TryGetValue(currentDialogueID, out var dialogue))
            {
                Debug.LogWarning($"현재 대화 ID {currentDialogueID}에 해당하는 데이터를 찾을 수 없습니다.");
                return;
            }

            bool hasNextDialogue = dialogue.nextDialogueID != 0; // 다음 대화가 있는지 확인
            bool isComplete = QuestManager.Instance.GetIsCompleteToNpc(npcID); // 퀘스트 완료 여부 확인

            dialoguePopUp.ShowDialogue(dialogue.dialogueText, hasNextDialogue, dialogue.hasQuest, isComplete); // 대화 팝업에 대화 표시

            if (hasNextDialogue)
                currentDialogueID = dialogue.nextDialogueID; // 다음 대화 ID 설정
        }

        int FindFirstDialogueID(int npcID, bool hasQuest)
        {
            foreach (var data in dataLoader.ItemsDict.Values)
            {
                if (data.npcID == npcID && data.hasQuest == hasQuest)
                {
                    return data.key; // 첫 번째 대화 ID 반환
                }
            }

            return NoDataID; // 데이터를 찾을 수 없으면 NoDataID 반환
        }

        int FindCompletedDialogueID(int npcID)
        {
            foreach (var data in dataLoader.ItemsDict.Values)
            {
                if (data.npcID == npcID && data.hasCompleteQuest)
                {
                    return data.key; // 완료된 대화 ID 반환
                }
            }

            return NoDataID; // 데이터를 찾을 수 없으면 NoDataID 반환
        }

        public void ShowQuestDialogue(bool isAccept)
        {
            if (!dataLoader.ItemsDict.TryGetValue(currentDialogueID, out DialogueData currentDialogue))
            {
                Debug.LogWarning($"현재 대화 ID {currentDialogueID}에 해당하는 데이터를 찾을 수 없습니다.");
                return;
            }

            DialogueData questDialogue = 
                dataLoader.ItemsDict.TryGetValue
                (isAccept ? currentDialogue.acceptDialogueID : currentDialogue.declineDialogueID,
                out var dialogue) ? dialogue : null; // 퀘스트 대화 데이터 가져오기

            bool hasNextDialogue = dialogue.nextDialogueID != EndOfDialogueID; // 다음 대화가 있는지 확인

            dialoguePopUp.ShowDialogue(dialogue.dialogueText, hasNextDialogue, dialogue.hasQuest); // 대화 팝업에 대화 표시

            currentDialogueID = hasNextDialogue ? currentDialogue.nextDialogueID : EndOfDialogueID; // 다음 대화 ID 설정
        }
    }
}