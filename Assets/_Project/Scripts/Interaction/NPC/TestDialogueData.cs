using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class TestDialogueData
{
    /// <summary>
    /// DialogueID
    /// </summary>
    public int key;

    /// <summary>
    /// NPCID
    /// </summary>
    public int npcID;

    /// <summary>
    /// DialogueText
    /// </summary>
    public string dialogueText;

    /// <summary>
    /// NextDialogueID
    /// </summary>
    public int nextDialogueID;

}
public class TestDialogueDataLoader
{
    public List<TestDialogueData> ItemsList { get; private set; }
    public Dictionary<int, TestDialogueData> ItemsDict { get; private set; }

    public TestDialogueDataLoader(string path = "JSON/TestDialogueData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, TestDialogueData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<TestDialogueData> Items;
    }

    public TestDialogueData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public TestDialogueData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
