using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum QuestType
{
    DungeonClear = 0,
    MonsterKill = 1,
    MaterialGather = 2,
}

[Serializable]
public class QuestData
{
    /// <summary>
    /// QuestID
    /// </summary>
    public int key;

    /// <summary>
    /// NPCID
    /// </summary>
    public int npcID;

    /// <summary>
    /// QuestDescription
    /// </summary>
    public string questDescription;

    /// <summary>
    /// QuestType
    /// </summary>
    public QuestType questType;

    /// <summary>
    /// EnabledQuestID
    /// </summary>
    public int enabledQuestID;

    /// <summary>
    /// TargetID
    /// </summary>
    public int targetID;

    /// <summary>
    /// TargetNum
    /// </summary>
    public int targetNum;

    /// <summary>
    /// RewardExp
    /// </summary>
    public int rewardExp;

    /// <summary>
    /// RewardItemID
    /// </summary>
    public int rewardItemID;

    /// <summary>
    /// RewardItemNum
    /// </summary>
    public int rewardItemNum;

}
public class QuestDataLoader
{
    public List<QuestData> ItemsList { get; private set; }
    public Dictionary<int, QuestData> ItemsDict { get; private set; }

    public QuestDataLoader(string path = "JSON/QuestData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, QuestData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<QuestData> Items;
    }

    public QuestData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public QuestData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
