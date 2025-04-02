using Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class AcceptQuestPopUp : UIPopup
    {
        [SerializeField]
        Button acceptButton;
        [SerializeField]
        Button declineButton;
        [SerializeField]
        Button rewardButton;

        [SerializeField]
        TextMeshProUGUI questNameText;
        [SerializeField]
        TextMeshProUGUI questDescriptionText;
        [SerializeField]
        TextMeshProUGUI questTargetText;
        [SerializeField]
        TextMeshProUGUI questRewardText;

        QuestData questData;

        void Start()
        {
            acceptButton.onClick.AddListener(OnClickAcceptButton);
            declineButton.onClick.AddListener(OnClickDeclineButton);
            rewardButton.onClick.AddListener(OnClickRewardButton);
        }

        public void SetQuestInfo(QuestData questData, bool isComplete)
        {
            this.questData = questData;

            questNameText.text = questData.questName;
            questDescriptionText.text = questData.questDescription;

            switch (questData.questType)
            {
                case QuestType.DungeonClear:
                    questTargetText.text = $"{questData.targetName} 클리어하기" + (isComplete ? "(완료)" : $"0/{questData.targetNum}");
                    break;
                case QuestType.MonsterKill:
                    questTargetText.text = $"{questData.targetName} 처치하기" + (isComplete ? "(완료)" : $"0/{questData.targetNum}");
                    break;
                case QuestType.MaterialGather:
                    questTargetText.text = $"{questData.targetName} 수집하기" + (isComplete ? "(완료)" : $"0/{questData.targetNum}");
                    break;
            }
            
            acceptButton.gameObject.SetActive(!isComplete);
            declineButton.gameObject.SetActive(!isComplete);
            rewardButton.gameObject.SetActive(isComplete);
            
            questRewardText.text = $"{questData.rewardExp} 경험치";
        }

        void OnClickAcceptButton()
        {
            DialogueManager.Instance.ShowQuestDialogue(true);

            if (questData != null)
                QuestManager.Instance.StartQuest(questData.key);

            ClosePopup();
        }

        void OnClickDeclineButton()
        {
            DialogueManager.Instance.ShowQuestDialogue(false);
            ClosePopup();
        }

        void OnClickRewardButton()
        {
            QuestManager.Instance.CompleteQuest(questData.key);
            DialogueManager.Instance.ShowQuestDialogue(true);
            ClosePopup();
        }
    }
}
