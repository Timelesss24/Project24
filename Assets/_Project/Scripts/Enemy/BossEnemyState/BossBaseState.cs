using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class BossBaseState : IState
    {
        // 현재 상태에서 제어할 적(Enemy) 객체
        protected readonly BossEnemy enemy;

        // 적의 애니메이션을 제어할 Animator 컴포넌트
        protected readonly Animator animator;

        // 애니메이션 상태 해시 (성능 최적화를 위해 Hash 값 사용)
        protected static readonly int IdleHash = Animator.StringToHash("Idle"); // Idle 상태
        protected static readonly int WalkHash = Animator.StringToHash("Walk"); // 달리기 상태
        protected static readonly int AttackFirstHash = Animator.StringToHash("Attack01"); // 공격 상태1
        protected static readonly int AttackSecondHash = Animator.StringToHash("Attack02"); // 공격 상태2
        protected static readonly int AttackThidHash = Animator.StringToHash("Attack03"); // 공격 상태3
        protected static readonly int DieHash = Animator.StringToHash("Die"); // 사망 상태

        // 애니메이션 전환 시 자연스러운 전환을 위한 CrossFade 지속 시간
        protected const float crossFadeDuration = 0.1f;

        /// <summary>
        /// EnemyBaseState 생성자:
        /// 적과 관련된 공통 데이터를 초기화합니다.
        /// </summary>
        /// <param name="enemy">현재 적 객체</param>
        /// <param name="animator">적의 애니메이션을 담당하는 Animator 컴포넌트</param>
        protected BossBaseState(BossEnemy enemy, Animator animator)
        {
            this.enemy = enemy;
            this.animator = animator;
        }

        /// <summary>
        /// 상태가 활성화(진입)될 때 실행되는 메서드.
        /// 상태 변화 시 기본적인 초기 동작을 정의합니다.
        /// </summary>
        public virtual void OnEnter()
        {
            // 상태 진입 시 디버그 메시지를 출력하여 현재 상태를 확인
            Debug.Log("Entering state: " + this.GetType().Name);
        }

        /// <summary>
        /// 상태가 활성화되어 있는 동안 매 프레임 호출됩니다.
        /// 상태 유지 중 필요한 동작을 정의할 수 있습니다.
        /// </summary>
        public virtual void Update()
        {
            // 기본 동작: 정의하지 않음 (각 상태에서 오버라이드 가능)
        }

        /// <summary>
        /// FixedUpdate: 물리적인 동작이 필요한 경우 호출됩니다.
        /// 상태 유지 중 필요한 물리 기반 행동 로직을 정의할 수 있습니다.
        /// </summary>
        public virtual void FixedUpdate()
        {
            // 기본 동작: 정의하지 않음 (각 상태에서 오버라이드 가능)
        }

        /// <summary>
        /// 상태가 비활성화(종료)될 때 실행되는 메서드.
        /// 상태 전환 시 필요한 정리 작업을 수행할 수 있습니다.
        /// </summary>
        public virtual void OnExit()
        {
            // 기본 동작: 정의하지 않음 (각 상태에서 오버라이드 가능)
        }
    }
}
