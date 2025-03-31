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

        public void StartDialogue(NPCInfo npcInfo, Transform npcTransform, Action onComplete = null)
        {
            this.npcID = npcInfo.ID;
            currentDialogueID = FindFirstDialogueID(npcInfo.ID);

            if (currentDialogueID == -1)
            {
                onComplete?.Invoke();
                Debug.LogWarning($"NPC {npcInfo.ID}�� ��ȭ �����͸� ã�� �� �����ϴ�.");
                return;
            }

            if(npcCamera != null)
            {
                npcCamera.transform.parent = npcTransform;
                npcCamera.transform.localPosition = new Vector3(0, 1.7f, 1f);
                npcCamera.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            }


            if(dialoguePopUp == null)
                dialoguePopUp = UIManager.Instance.ShowPopup<DialoguePopUp>();

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
                Debug.LogWarning($"���� ��ȭ ID {currentDialogueID}�� �ش��ϴ� �����͸� ã�� �� �����ϴ�.");
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
