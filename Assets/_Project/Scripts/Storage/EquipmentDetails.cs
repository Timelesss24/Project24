using System;
using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "New EquipItem", menuName = "Inventory/EquipItem")]
    [Serializable]
    public class EquipmentDetails : ItemDetails
    {
        public GameObject EquipmentPrefab;
        
        [Tooltip("Spawn Transform")]
        public Vector3 LocalPosition;  // ✅ 소문자 필드명, 접근 쉬움

        [Tooltip("Spawn Transform")]
        public Vector3 LocalRotation;
        
        public virtual void OnValidate()
        {
            ItemType = ItemType.Equipment;
        }
        public HumanBodyBones EquipHolder()
        {
            return EquipmentType switch
            {
                EquipmentType.Helmet => HumanBodyBones.Head,
                EquipmentType.Armor => HumanBodyBones.Chest,
                EquipmentType.Boots => HumanBodyBones.Hips,
                EquipmentType.Weapon => HumanBodyBones.RightHand,
                _ => HumanBodyBones.Chest,
            };
        }
        
        public override Item Create(int quantity)
        {
            return new EquipmentItem(this, quantity);
        }
    }
}