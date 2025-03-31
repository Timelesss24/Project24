using UnityEngine;

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
        public string ItemName;
        public string ItemDescription;
        public Sprite ItemIcon;
        public ItemType itemType;

        public int Stack = 0;

        public abstract void OnUseItem();
    }
}
