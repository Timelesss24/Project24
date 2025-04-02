using Scripts.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class QuestPopUp : UIPopup
    {
        [SerializeField]
        Button closeButton;
        [SerializeField]
        TextMeshProUGUI nameText;
        [SerializeField]
        TextMeshProUGUI descriptionText;
        [SerializeField]
        TextMeshProUGUI progressText;
        [SerializeField]
        TextMeshProUGUI rewardText;

        [SerializeField]
        GameObject infoObj;
        [SerializeField]
        GameObject slotPrefab;
        [SerializeField]
        Transform slotParent;

        QuestManager questManager;

        List<QuestData> questDataList = new List<QuestData>();
        List<QuestSlot> questSlots = new List<QuestSlot>();

        void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);

            questManager = QuestManager.Instance;

            LoadActiveQuests();

            if (questDataList.Count > 0)
                CreateQuestSlots();
        }

        void LoadActiveQuests()
        {
            foreach (ActiveQuestInfo quest in questManager.ActiveQuestList)
            {
                questDataList.Add(questManager.GetQuestData(quest.questID));
            }
        }

        void OnClickCloseButton()
        {
            ClosePopup();
        }

        void CreateQuestSlots()
        {
            foreach (QuestData questData in questDataList)
            {
                QuestSlot slot = Instantiate(slotPrefab, slotParent).GetComponent<QuestSlot>();
                questSlots.Add(slot);
                slot.SetSlot(questData, this);
            }
        }

        public void SetQuestInfo(QuestData questData)
        {
            infoObj.SetActive(true);

            nameText.text = questData.questName;
            descriptionText.text = questData.questDescription;

            ActiveQuestInfo activeQuest = questManager.ActiveQuestList.Find(q => q.questID == questData.key);
            int currentProgress = activeQuest != null ? activeQuest.progress : 0;
            bool isClear = activeQuest.progress >= activeQuest.goal;

            switch (questData.questType)
            {
                case QuestType.DungeonClear:
                    progressText.text = $"{questData.targetName} 클리어하기" + (isClear ? "(완료)" : $"{currentProgress}/{questData.targetNum}");
                    break;
                case QuestType.MonsterKill:
                    progressText.text = $"{questData.targetName} 처치하기" + (isClear ? "(완료)" : $"{currentProgress}/{questData.targetNum}");
                    break;
                case QuestType.MaterialGather:
                    progressText.text = $"{questData.targetName} 수집하기" + (isClear ? "(완료)" : $"{currentProgress}/{questData.targetNum}");
                    break;
            }

            rewardText.text = $"{questData.rewardExp} 경험치";
        }
    }
}
