using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace Timelesss
{
    public class QuestManager : PersistentSingleton<QuestManager>
    {
        private QuestDataLoader questDataLoader;

        private Dictionary<int, QuestData> questDict = new Dictionary<int, QuestData>();

        private List<ActiveQuestInfo> activeQuestList = new List<ActiveQuestInfo>();
        public List<ActiveQuestInfo> ActiveQuestList { get { return activeQuestList; } }

        private List<int> completeQuestList = new List<int>();
        public List<int> CompleteQuestList { get { return completeQuestList; } }

        private void Start()
        {
            questDataLoader = new QuestDataLoader();
            questDict = questDataLoader.ItemsDict;
        }

        public int GetQuestID(int npcID)
        {
            foreach (var quest in questDict.Values)
            {
                if (quest.npcID == npcID &&
                     !activeQuestList.Exists(q => q.questID == quest.key) &&
                     !completeQuestList.Contains(quest.key) &&
                     (quest.enabledQuestID == 0 || completeQuestList.Contains(quest.enabledQuestID)))
                {
                    return quest.key;
                }
            }

            return 0;
        }

        public QuestData GetQuestData(int questID)
        {
            if (questDict.TryGetValue(questID, out var questData))
            {
                return questData;
            }
            return null; 
        }

        public void StartQuest(int questID)
        {
            if (!activeQuestList.Exists(q => q.questID == questID) && questDict.ContainsKey(questID))
            {
                QuestData questData = GetQuestData(questID);
                if (questData != null)
                {
                    activeQuestList.Add(new ActiveQuestInfo(questID, questData.targetNum));
                    Debug.Log($"퀘스트 시작: {questDict[questID].questDescription}");
                }
            }
            else
            {
                Debug.LogWarning($"퀘스트를 시작할 수 없습니다. (퀘스트 ID: {questID})");
            }
        }

        public void CompleteQuest(int questID)
        {
            ActiveQuestInfo activeQuest = activeQuestList.Find(q => q.questID == questID);

            if (activeQuest != null)
            {
                activeQuestList.Remove(activeQuest);
                completeQuestList.Add(questID);

                if (questDict.TryGetValue(questID, out var completedQuest))
                {
                    Debug.Log($"퀘스트 완료: {completedQuest.questDescription}");
                    RewardPlayer(completedQuest.rewardExp, completedQuest.rewardItemID, completedQuest.rewardItemNum);
                }
                else
                {
                    Debug.LogWarning($"완료하려는 퀘스트 데이터를 찾을 수 없습니다. (퀘스트 ID: {questID})");
                }
            }
            else
            {
                Debug.LogWarning($"완료하려는 퀘스트가 활성 상태가 아닙니다. (퀘스트 ID: {questID})");
            }
        }

        private void RewardPlayer(int exp, int itemId, int itemNum)
        {
            Debug.Log($"보상 지급: 경험치 {exp}, 아이템 ID {itemId}, 수량 {itemNum}");
        }
    }
}
