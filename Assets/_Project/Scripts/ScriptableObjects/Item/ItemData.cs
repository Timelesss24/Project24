using UnityEngine;

namespace Timelesss
{
    public abstract class ItemData : ScriptableObject
    {
        public string ItemName;
        public string ItemDescription;
        public Sprite ItemIcon;

        public abstract string ItemType {  get; }

        public int Stack = 0;
    }
}
