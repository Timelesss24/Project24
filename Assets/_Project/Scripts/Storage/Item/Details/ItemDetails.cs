using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Timelesss
{
    public enum ItemType
    {
        Consumable,
        Equipment,
        Quest,
    }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    [Serializable]
    public class ItemDetails : ScriptableObject
    {
        public string Name;
        public int MaxStack = 1;
        public SerializableGuid Id = SerializableGuid.NewGuid();

        public ItemType ItemType;
        public GameObject DropPrefab;
        public EquipmentType EquipmentType;
        
        public GameObject EquipmentPrefab;
        
        //Consume
        public int RestoreValue;
        
        // Equip
        [Tooltip("Spawn Transform")]
        public Vector3 LocalPosition;  

        [Tooltip("Spawn Transform")]
        public Vector3 LocalRotation;
        
        [Tooltip("각 공격과 해당 조건에 대한 세부 정보를 포함하는 공격 데이터 컨테이너입니다.")]
        public AttackContainer AttacksContainer;

        [Tooltip("무기에 특화된 움직임 애니메이션을 관리하기 위한 애니메이터 오버라이드 컨트롤러입니다.")] 
        public AnimatorOverrideController OverrideController;
        
        void AssignNewGuid()
        {
            Id = SerializableGuid.NewGuid();
        }

        public Sprite Icon;

        [TextArea]
        public string Description;
        
        // 초기화 메서드
        public void InIt()
        {
            // 모든 공격 데이터를 탐색하며 각 AttackSlot에 해당 컨테이너를 설정
            foreach (var attack in AttacksContainer.Attacks)
                attack.Container = AttacksContainer;
        }

        public  Item Create(int quantity)
        {
            return ItemType switch
            {
                ItemType.Quest => new Item(this, quantity),
                ItemType.Equipment => new EquipmentItem(this, quantity),
                ItemType.Consumable => new ConsumableItem(this, quantity),
                _ => new Item(this, quantity),
            };

        }
    }

}