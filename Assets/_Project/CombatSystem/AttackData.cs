using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    // 공격 데이터를 생성하기 위한 ScriptableObject
    [CreateAssetMenu(menuName = "Combat/Create Attack")]
    public class AttackData : ScriptableObject
    {
        [field: SerializeField]
        public AnimationClip Clip { get; private set; } // 공격에 사용되는 애니메이션 클립

        [field: SerializeField]
        public float ImpactStartTime { get; private set; } // 공격 효과가 시작되는 시간

        [field: SerializeField]
        public float ImpactEndTime { get; private set; } // 공격 효과가 끝나는 시간
    }

    [Serializable]
    public class AttackContainer
    {
        [Tooltip("이 공격에 사용할 수 있는 공격 데이터들의 리스트입니다.")]
        [field: SerializeField]
        public List<AttackSlot> Attacks { get; private set; } // 공격 데이터 슬롯 리스트
    }

    [Serializable]
    public class AttackSlot
    {
        [Tooltip("특정 공격에 대한 데이터입니다.")]
        [field: SerializeField]
        public AttackData Attack { get; private set; } // 특정 공격 데이터

        [NonSerialized]
        public AttackContainer Container; // 소속된 AttackContainer (런타임 중에만 사용)
    }

    // 무기 데이터를 생성하기 위한 ScriptableObject
    [CreateAssetMenu(menuName = "Combat/Create Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Tooltip("각 공격과 해당 조건에 대한 세부 정보를 포함하는 공격 데이터 컨테이너입니다.")]
        [field: SerializeField]
        public AttackContainer AttacksContainer { get; private set; } // 공격 데이터 컨테이너

        [Tooltip("무기에 특화된 움직임 애니메이션을 관리하기 위한 애니메이터 오버라이드 컨트롤러입니다.")]
        [field: SerializeField]
        public AnimatorOverrideController OverrideController { get; private set; } // 애니메이터 오버라이드 컨트롤러

        // 초기화 메서드
        public void InIt()
        {
            // 모든 공격 데이터를 탐색하며 각 AttackSlot에 해당 컨테이너를 설정
            foreach (var attack in AttacksContainer.Attacks)
                attack.Container = AttacksContainer;
        }
    }
}