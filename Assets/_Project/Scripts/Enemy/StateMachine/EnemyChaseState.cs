using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Timelesss
{
    public class EnemyChaseState : EnemyBaseState
    {
        // 적이 추적할 때 사용하는 NavMeshAgent 컴포넌트
        readonly NavMeshAgent agent;

        // 추적 대상인 플레이어의 Transform
        readonly Transform target;

        /// <summary>
        /// EnemyChaseState의 생성자.
        /// 특정 적 객체와 이를 제어할 Animator, NavMeshAgent, 추적 대상(Target)을 초기화합니다.
        /// </summary>
        /// <param name="enemy">현재 적 객체</param>
        /// <param name="animator">적의 애니메이션을 제어하는 Animator</param>
        /// <param name="agent">적의 이동 및 경로 탐색을 담당하는 NavMeshAgent</param>
        /// <param name="target">플레이어의 Transform</param>
        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform target)
            : base(enemy, animator) // 기본 상태 클래스의 생성자 호출
        {
            this.agent = agent;
            this.target = target;
        }

        /// <summary>
        /// 상태가 활성화(진입)될 때 실행되는 함수입니다.
        /// 추적 상태에 필요한 초기 설정을 수행합니다.
        /// </summary>
        public override void OnEnter()
        {
            // Run 애니메이션 실행 (CrossFade: 자연스러운 애니메이션 전환)
            animator.CrossFade(RunHash, crossFadeDuration);
        }

        /// <summary>
        /// 상태가 활성화되어 있는 동안 매 프레임 호출됩니다.
        /// 대상의 위치를 NavMeshAgent에 전달하여 적이 추적하도록 설정합니다.
        /// </summary>
        public override void Update()
        {
            // 타겟의 현재 위치를 NavMeshAgent가 이동하도록 설정
            agent.SetDestination(target.position);
        }

        /// <summary>
        /// 상태가 비활성화(종료)될 때 실행되는 함수입니다.
        /// 추적 동작을 멈추고 대상 경로를 초기화합니다.
        /// </summary>
        public override void OnExit()
        {
            // 추적을 멈추고 NavMeshAgent의 현재 경로를 초기화
            agent.ResetPath();
        }
    }
}
