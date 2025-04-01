using System;
using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Timelesss
{
    public class DialogueManager : UnityUtils.Singleton<DialogueManager>
    {
        private DialogueDataLoader dataLoader;
        public int currentDialogueID;
        private int npcID;

        private DialoguePopUp dialoguePopUp;

        private Camera npcCamera;

        private const int EndOfDialogueID = 0;
        private const int NoDataID = -1;
        
        private void Start()
        {
            dataLoader = new DialogueDataLoader();

            npcCamera = GetComponentInChildren<Camera>();
        }

        public void StartDialogue(NPCInfo npcInfo, Transform npcTransform, Action onComplete = null)
        {
            this.npcID = npcInfo.ID;

            int questID = QuestManager.Instance.GetQuestID(npcInfo.ID);

            bool hasCompleted = QuestManager.Instance.GetIsCompleteToNpc(npcInfo.ID);
            
            currentDialogueID = hasCompleted ? 
                FindCompletedDialogueID(npcInfo.ID) : FindFirstDialogueID(npcInfo.ID, questID != 0);

            if (currentDialogueID == NoDataID)
            {
                onComplete?.Invoke();
                Debug.LogWarning($"NPC {npcInfo.ID}의 대화 데이터를 찾을 수 없습니다.");
                return;
            }

            if (npcCamera != null)
            {
                npcCamera.transform.parent = npcTransform;
                npcCamera.transform.localPosition = new Vector3(0, 1.7f, 1f);
                npcCamera.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            }

            if (dialoguePopUp == null)
            {
                dialoguePopUp = UIManager.Instance.ShowPopup<DialoguePopUp>();
                dialoguePopUp.SetNpcID(npcInfo.ID);
            }

            dialoguePopUp.Show();
            dialoguePopUp.SetNpcNameText(npcInfo.Name);

            ShowCurrentDialogue();

            StartCoroutine(TrackingDialogue(onComplete));
        }

        private IEnumerator TrackingDialogue(Action onComplete = null)
        {
            yield return new WaitWhile(() => dialoguePopUp != null);

            onComplete?.Invoke();
        }

        public void ShowCurrentDialogue()
        {
            if (!dataLoader.ItemsDict.TryGetValue(currentDialogueID, out var dialogue))
            {
                Debug.LogWarning($"현재 대화 ID {currentDialogueID}에 해당하는 데이터를 찾을 수 없습니다.");
                return;
            }

            bool hasNextDialogue = dialogue.nextDialogueID != 0;
            bool isComplete = QuestManager.Instance.GetIsCompleteToNpc(npcID);

            dialoguePopUp.ShowDialogue(dialogue.dialogueText, hasNextDialogue, dialogue.hasQuest, isComplete);

            if (hasNextDialogue)
                currentDialogueID = dialogue.nextDialogueID;
        }

        private int FindFirstDialogueID(int npcID, bool hasQuest)
        {
            foreach (var data in dataLoader.ItemsDict.Values)
            {
                if (data.npcID == npcID && data.hasQuest == hasQuest)
                {
                    return data.key;
                }
            }

            return NoDataID;
        }

        private int FindCompletedDialogueID(int npcID)
        {
            foreach (var data in dataLoader.ItemsDict.Values)
            {
                if (data.npcID == npcID && data.hasCompleteQuest)
                {
                    return data.key;
                }
            }

            return NoDataID;
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
                out var dialogue) ? dialogue : null;

            bool hasNextDialogue = dialogue.nextDialogueID != EndOfDialogueID;

            dialoguePopUp.ShowDialogue(dialogue.dialogueText, hasNextDialogue, dialogue.hasQuest);

            currentDialogueID = hasNextDialogue ? currentDialogue.nextDialogueID : EndOfDialogueID;
        }
    }
}
