using UnityEngine;

namespace Timelesss
{
    
    public abstract class ItemData : ScriptableObject
    {
          [field: SerializeField] public string ItemName { get; protected set; }
          [field: SerializeField] public string ItemDescription{ get; protected set; }
          [field: SerializeField] public Sprite ItemIcon{ get; protected set; }
          public abstract ItemType Type{ get; }
          public abstract bool CanStack{ get; }
          
          public abstract IItemStrategy CreateStrategy();
    }
}
