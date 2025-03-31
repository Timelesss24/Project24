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
        private List<int> activeQuests = new List<int>();
        private List<int> completeQuests = new List<int>();

        private void Start()
        {
            questDataLoader = new QuestDataLoader();
            questDict = questDataLoader.ItemsDict;
        }

        public int GetQuest(int npcID)
        {
            foreach (var quest in questDict.Values)
            {
                if (quest.npcID == npcID &&
                    !activeQuests.Contains(quest.key) &&
                    !completeQuests.Contains(quest.key) &&
                    (quest.enabledQuestID == 0 || completeQuests.Contains(quest.enabledQuestID)))
                {
                    StartQuest(quest.key);
                    return quest.key;
                }
            }

            return 0;
        }

        public void StartQuest(int questID)
        {
            if (!activeQuests.Contains(questID) && questDict.ContainsKey(questID))
            {
                activeQuests.Add(questID);
                Debug.Log($"퀘스트 시작: {questDict[questID].questDescription}");
            }
            else
            {
                Debug.LogWarning($"퀘스트를 시작할 수 없습니다. (퀘스트 ID: {questID})");
            }
        }

        public void CompleteQuest(int questID)
        {
            if (activeQuests.Contains(questID))
            {
                activeQuests.Remove(questID);
                completeQuests.Add(questID);

                if (questDict.TryGetValue(questID, out var completedQuest))
                {
                    Debug.Log($"퀘스트 완료: {completedQuest.questDescription}");
                    RewardPlayer(completedQuest.rewardExp, completedQuest.rewardItemID, completedQuest.rewardItemNum);

                    if (completedQuest.enabledQuestID != 0)
                    {
                        Debug.Log($"후속 퀘스트 활성화: {completedQuest.enabledQuestID}");
                        StartQuest(completedQuest.enabledQuestID);
                    }
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
            Debug.Log($"플레이어에게 보상 지급: 경험치 {exp}, 아이템 ID {itemId}, 수량 {itemNum}");
        }
    }
}
