using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Timelesss
{
    public class DialoguePopUp : UIPopup
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private Camera NPCCamera;

        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI npcNameText;


        private Coroutine typewriterCoroutine;
        private float typewriterSpeed = 0.05f;

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

        public void ShowDialogue(string text, bool hasNextDialogue, bool hasQuest)
        {
            nextButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);

            typewriterCoroutine = StartCoroutine(ChangeDialogueTextCoroutine(text, hasNextDialogue, hasQuest));
        }

        private IEnumerator ChangeDialogueTextCoroutine(string text, bool hasNextDialogue, bool hasQuest)
        {
            dialogueText.text = "";

            text = text.Replace("모험가", $"{playerInfo.GetName()}");
            Debug.Log(text);

            foreach (char letter in text)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typewriterSpeed);
            }

            if (hasNextDialogue)
            {
                nextButton.gameObject.SetActive(true);
            }
            else if (!hasNextDialogue && hasQuest)
            {
                int questID = QuestManager.Instance.GetQuestID(npcID);
                if (questID != 0)
                {
                    Debug.Log("Has Quest");
                    QuestData questData = QuestManager.Instance.GetQuestData(questID);
                    AcceptQuestPopUp popUp = UIManager.Instance.ShowPopup<AcceptQuestPopUp>();
                    popUp.SetQuestInfo(questData);
                }

            }
            else if (!hasNextDialogue && !hasQuest)
            {
                closeButton.gameObject.SetActive(true);
            }
        }

        private void OnClickNextButton()
        {
            Debug.Log("Next Button");
            DialogueManager.Instance.ShowCurrentDialogue();
        }

        private void OnClickCloseButton()
        {
            ClosePopup();
        }

        public void SetNPCNameText(string name) => npcNameText.text = name;
    }
}
