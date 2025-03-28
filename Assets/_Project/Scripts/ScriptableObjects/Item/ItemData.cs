using UnityEngine;

namespace Timelesss
{
    public abstract class ItemData : ScriptableObject
    {
        public string ItemName;
        public string ItemDescription;
        public Sprite ItemIcon;
    }
}
