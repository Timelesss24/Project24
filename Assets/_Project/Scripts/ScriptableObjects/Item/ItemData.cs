﻿using UnityEngine;

namespace Timelesss
{
    public enum ItemType
    {
        UseableItem,
        EquipableItem,
        QuestItem,
    }
    
    public abstract class ItemData : ScriptableObject
    {
          [field: SerializeField] public string ItemName { get; protected set; }
          [field: SerializeField] public string ItemDescription{ get; protected set; }
          [field: SerializeField] public Sprite ItemIcon{ get; protected set; }
          public abstract ItemType ItemType{ get; }

        public int Stack = 0;

        public abstract void OnUseItem();
    }
}
