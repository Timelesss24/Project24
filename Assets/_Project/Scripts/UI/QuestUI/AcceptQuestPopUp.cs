using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class AcceptQuestPopUp : UIPopup
    {
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;

        [SerializeField] private TextMeshProUGUI questNameText;
        [SerializeField] private TextMeshProUGUI questDescriptionText;
        [SerializeField] private TextMeshProUGUI questTargetText;
        [SerializeField] private TextMeshProUGUI questRewardText;

        private QuestData questData;

        private void Start()
        {
            acceptButton.onClick.AddListener(OnClickAcceptButton);
            declineButton.onClick.AddListener(OnClickDeclineButton);
        }

        public void SetQuestInfo(QuestData questData)
        {
            this.questData = questData;

            questNameText.text = questData.questName;
            questDescriptionText.text = questData.questDescription;

            switch (questData.questType)
            {
                case QuestType.DungeonClear:
                    questTargetText.text = $"{questData.targetName} 클리어하기 0/{questData.targetNum}";
                    break;
                case QuestType.MonsterKill:
                    questTargetText.text = $"{questData.targetName} 처치하기 0/{questData.targetNum}";
                    break;
                case QuestType.MaterialGather:
                    questTargetText.text = $"{questData.targetName} 수집하기 0/{questData.targetNum}";
                    break;
            }

            questRewardText.text = $"{questData.rewardExp} 경험치";
        }

        private void OnClickAcceptButton()
        {
            DialogueManager.Instance.ShowQuestDialogue(true);

            if (questData != null)
                QuestManager.Instance.StartQuest(questData.key);

            ClosePopup();
        }

        private void OnClickDeclineButton()
        {
            DialogueManager.Instance.ShowQuestDialogue(false);
            ClosePopup();
        }
    }
}
