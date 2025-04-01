using UnityEngine;

namespace Timelesss
{
    // 무기 데이터를 생성하기 위한 ScriptableObject
    [CreateAssetMenu(menuName = "Combat/Create Weapon")]
    public class WeaponData : EquipItemData
    {
        [Tooltip("무기 오브젝트")]
        [field: SerializeField] public GameObject WeaponModel { get; private set; } // 애니메이터 오버라이드 컨트롤러
        [field: SerializeField] public Vector3 LocalPosition { get; private set; } // 스폰 Transform
        [field: SerializeField] public Vector3 LocalRotation { get; private set; } // 스폰 Transform
        
        [Tooltip("각 공격과 해당 조건에 대한 세부 정보를 포함하는 공격 데이터 컨테이너입니다.")]
        [field: SerializeField]
        public AttackContainer AttacksContainer { get; private set; } // 공격 데이터 컨테이너

        [Tooltip("무기에 특화된 움직임 애니메이션을 관리하기 위한 애니메이터 오버라이드 컨트롤러입니다.")]
        [field: SerializeField]
        public AnimatorOverrideController OverrideController { get; private set; } // 애니메이터 오버라이드 컨트롤러
        
        public override EquipType EquipType => EquipType.Sword;
        // 초기화 메서드
        public void InIt()
        {
            // 모든 공격 데이터를 탐색하며 각 AttackSlot에 해당 컨테이너를 설정
            foreach (var attack in AttacksContainer.Attacks)
                attack.Container = AttacksContainer;
        }
        
    }
}