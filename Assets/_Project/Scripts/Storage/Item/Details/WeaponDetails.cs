using System;
using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
    [Serializable]
    public class WeaponDetails : EquipmentDetails
    {
        [Tooltip("각 공격과 해당 조건에 대한 세부 정보를 포함하는 공격 데이터 컨테이너입니다.")]
        public AttackContainer AttacksContainer;

        [Tooltip("무기에 특화된 움직임 애니메이션을 관리하기 위한 애니메이터 오버라이드 컨트롤러입니다.")] 
        public AnimatorOverrideController OverrideController;
        
        public override void OnValidate()
        {
            base.OnValidate();
            EquipmentType = EquipmentType.Weapon;
        }
        // 초기화 메서드
        public void InIt()
        {
            // 모든 공격 데이터를 탐색하며 각 AttackSlot에 해당 컨테이너를 설정
            foreach (var attack in AttacksContainer.Attacks)
                attack.Container = AttacksContainer;
        }
    }
}