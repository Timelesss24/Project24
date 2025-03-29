using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace Timelesss
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private TestDialogueDataLoader dataLoader;
        private int currentDialogueID;
        private int npcID;

        private DialoguePopUp dialoguePopUp;

        private Camera npcCamera;

        private void Start()
        {
            dataLoader = new TestDialogueDataLoader();

            npcCamera = GetComponentInChildren<Camera>();
        }

        public void StartDialogue(NPCInfo npcInfo, Transform npcTransform)
        {
            this.npcID = npcInfo.ID;
            currentDialogueID = FindFirstDialogueID(npcInfo.ID);

            if (currentDialogueID == -1)
            {
                Debug.LogWarning($"NPC {npcInfo.ID}의 대화 데이터를 찾을 수 없습니다.");
                return;
            }

            if(npcCamera != null)
            {
                npcCamera.transform.parent = npcTransform;
                npcCamera.transform.localPosition = new Vector3(0, 1.7f, 1f);
            }


            if(dialoguePopUp == null)
                dialoguePopUp = UIManager.Instance.ShowPopup<DialoguePopUp>();

            dialoguePopUp.Show();

            ShowCurrentDialogue();
        }

        public void ShowCurrentDialogue()
        {
            if (!dataLoader.ItemsDict.TryGetValue(currentDialogueID, out var dialogue))
            {
                Debug.LogWarning($"현재 대화 ID {currentDialogueID}에 해당하는 데이터를 찾을 수 없습니다.");
                return;
            }

            bool hasNextDialogue = dialogue.nextDialogueID != 0;

            dialoguePopUp.ShowDialogue(dialogue.dialogueText, hasNextDialogue);

            if (hasNextDialogue)
            {
                currentDialogueID = dialogue.nextDialogueID; 
            }
            else
            {
                ResetToFirstDialogue();
            }
        }

        private int FindFirstDialogueID(int npcID)
        {
            foreach (var data in dataLoader.ItemsDict.Values)
            {
                if (data.npcID == npcID)
                {
                    return data.key;
                }
            }

            return -1;
        }

        private void ResetToFirstDialogue()
        {
            currentDialogueID = FindFirstDialogueID(npcID);
        }
    }
}
