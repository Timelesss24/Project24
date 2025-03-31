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
        private int currentDialogueID;
        private int npcID;

        private DialoguePopUp dialoguePopUp;

        private Camera npcCamera;

        private void Start()
        {
            dataLoader = new DialogueDataLoader();

            npcCamera = GetComponentInChildren<Camera>();
        }

        public void StartDialogue(NPCInfo npcInfo, Transform npcTransform, Action onComplete = null)
        {
            this.npcID = npcInfo.ID;

            int questID = QuestManager.Instance.GetQuestID(npcInfo.ID);

            currentDialogueID = FindFirstDialogueID(npcInfo.ID, questID != 0);


            if (currentDialogueID == -1)
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
            dialoguePopUp.SetNPCNameText(npcInfo.Name);

            ShowCurrentDialogue();

            StartCoroutine(TrackingDialogue(onComplete));
        }

        IEnumerator TrackingDialogue(Action onComplete = null)
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

            dialoguePopUp.ShowDialogue(dialogue.dialogueText, hasNextDialogue, dialogue.hasQuest);

            if (hasNextDialogue)
            {
                currentDialogueID = dialogue.nextDialogueID;
            }
            else
            {
                ResetToFirstDialogue();
            }
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

            return -1;
        }

        private void ResetToFirstDialogue()
        {
            currentDialogueID = 0;
        }
    }
}
