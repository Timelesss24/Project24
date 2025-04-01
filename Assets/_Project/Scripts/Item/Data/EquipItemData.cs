using UnityEngine;

namespace Timelesss
{
    public enum EquipType
    {
        Null,
        Sword,
        Helmet,
    }
    [CreateAssetMenu(fileName = "New Equip Item Data", menuName = "Item/Equip Item Data")]
    public class EquipItemData : ItemData
    {
       // public Material Material;
        [field: SerializeField] public virtual EquipType EquipType { get; set; }
        [field: SerializeField] public float EquipValue {get; protected set;}
        [field: SerializeField] public GameObject EquipPrefab { get; protected set; }
        

        public override ItemType ItemType => ItemType.EquipableItem;
        public override bool CanStack => false;
        public override IItemStrategy CreateStrategy()
        {
            return new EquipStrategy(this);
        }

        public HumanBodyBones GetHolder()
        {
            return EquipType switch
            {
                EquipType.Sword => HumanBodyBones.RightHand,
                EquipType.Helmet => HumanBodyBones.Head,
                _ => HumanBodyBones.Hips
            };
        }
    }
}
