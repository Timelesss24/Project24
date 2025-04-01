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
        public Vector3 LocalPosition;  

        [Tooltip("Spawn Transform")]
        public Vector3 LocalRotation;
        
        public virtual void OnValidate()
        {
            ItemType = ItemType.Equipment;
        }
        
        public override Item Create(int quantity)
        {
            return new EquipmentItem(this, quantity);
        }
    }
}