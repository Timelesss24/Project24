using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DialogueData
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

    /// <summary>
    /// HasQuest
    /// </summary>
    public bool hasQuest;

    /// <summary>
    /// HasCompleteQuest
    /// </summary>
    public bool hasCompleteQuest;

    /// <summary>
    /// AcceptDialogueID
    /// </summary>
    public int acceptDialogueID;

    /// <summary>
    /// DeclineDialogueID
    /// </summary>
    public int declineDialogueID;

}
public class DialogueDataLoader
{
    public List<DialogueData> ItemsList { get; private set; }
    public Dictionary<int, DialogueData> ItemsDict { get; private set; }

    public DialogueDataLoader(string path = "JSON/DialogueData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, DialogueData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<DialogueData> Items;
    }

    public DialogueData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public DialogueData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}