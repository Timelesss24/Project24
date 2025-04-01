using Scripts.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class QuestPopUp : UIPopup
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI rewardText;

        [SerializeField] private GameObject infoObj;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotParent;

        private QuestManager questManager;

        private List<QuestData> questDataList = new List<QuestData>();
        private List<QuestSlot> questSlots = new List<QuestSlot>();

        private void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);

            questManager = QuestManager.Instance;

            LoadActiveQuests();

            if (questDataList.Count > 0)
                CreateQuestSlots();
        }

        private void LoadActiveQuests()
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

        private void CreateQuestSlots()
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
