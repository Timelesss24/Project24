using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Timelesss
{
    public class DialoguePopUp : UIPopup
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI npcNameText;

        private Coroutine typewriterCoroutine;
        private const float TypeWriterSpeed = 0.05f;

        private PlayerInfo playerInfo;

        private int npcID;

        private void Awake()
        {
            playerInfo = FindObjectOfType<PlayerInfo>();

            if (playerInfo == null)
            {
                Debug.Log("playerInfo를 찾을 수 없습니다.");
            }

            nextButton.onClick.AddListener(OnClickNextButton);
            closeButton.onClick.AddListener(OnClickCloseButton);
        }

        public void SetNpcID(int id) => npcID = id;

        public void ShowDialogue(string text, bool hasNextDialogue, bool hasQuest, bool isComplete = false)
        {
            nextButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);

            typewriterCoroutine =
                StartCoroutine(ChangeDialogueTextCoroutine(text, hasNextDialogue, hasQuest, isComplete));
        }

        private IEnumerator ChangeDialogueTextCoroutine(string text, bool hasNextDialogue, bool hasQuest,
            bool isComplete = false)
        {
            dialogueText.text = "";

            text = text.Replace("모험가", $"{playerInfo.GetName()}");

            foreach (char letter in text)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(TypeWriterSpeed);
            }

            if (hasNextDialogue)
            {
                nextButton.gameObject.SetActive(true);
            }
            else
            {
                HandleEndOfDialogue(hasQuest, isComplete);
            }
        }

        private void HandleEndOfDialogue(bool hasQuest, bool isComplete)
        {
            if (isComplete)
            {
                ShowQuestPopup(isComplete: true);
            }
            else if (hasQuest)
            {
                ShowQuestPopup(isComplete: false);
            }
            else
            {
                closeButton.gameObject.SetActive(true);
            }
        }

        private void ShowQuestPopup(bool isComplete)
        {
            int questID = isComplete
                ? QuestManager.Instance.FindCompletedQuestID(npcID)
                : QuestManager.Instance.GetQuestID(npcID);

            if (questID == 0) return;

            QuestData questData = QuestManager.Instance.GetQuestData(questID);

            if (questData == null) return;

            AcceptQuestPopUp popUp = UIManager.Instance.ShowPopup<AcceptQuestPopUp>();
            popUp.SetQuestInfo(questData, isComplete);
        }

        private void OnClickNextButton()
        {
            DialogueManager.Instance.ShowCurrentDialogue();
        }

        private void OnClickCloseButton()
        {
            ClosePopup();
        }

        public void SetNpcNameText(string name) => npcNameText.text = name;
    }
}