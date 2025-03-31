using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Timelesss
{
    public class EnemyAttackState : EnemyBaseState
    {
        // 적의 이동과 경로 탐색을 담당하는 NavMeshAgent 컴포넌트
        readonly NavMeshAgent agent;

        // 공격 대상인 플레이어의 Transform
        readonly Transform target;

        /// EnemyAttackState 생성자:
        /// 적 개체, 애니메이터, NavMeshAgent, 공격 타겟을 초기화합니다.
        /// </summary>
        /// <param name="enemy">현재 적 객체</param>
        /// <param name="animator">적의 애니메이션을 제어하는 Animator</param>
        /// <param name="agent">적의 이동과 경로 탐색을 담당하는 NavMeshAgent</param>
        /// <param name="target">적이 공격할 대상 (플레이어의 Transform)</param>
        public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform target)
            : base(enemy, animator) // 부모 클래스의 생성자 호출
        {
            this.agent = agent; // NavMeshAgent 초기화
            this.target = target; // 공격 대상 초기화
        }

        /// 상태가 활성화(진입)될 때 실행됩니다.
        /// 공격 상태 진입 시, 공격 애니메이션을 실행합니다.
        public override void OnEnter()
        {
            // Attack 애니메이션을 부드럽게 시작 (CrossFade 사용)
            animator.CrossFade(AttackHash, crossFadeDuration);
            Debug.Log("공격");
        }

        /// <summary>
        /// 상태가 활성화되어 있는 동안 매 프레임 호출됩니다.
        /// 적은 공격 대상을 바라보며, 공격 동작을 수행합니다.
        /// </summary>
        public override void Update()
        {
            // 타겟의 위치를 NavMeshAgent의 높이와 동일하게 보정한 위치로 설정
            var targetPosition = new Vector3(target.position.x, agent.transform.position.y, target.position.z);

            // 적(NavMeshAgent)이 타겟을 바라보도록 설정
            agent.transform.LookAt(targetPosition);

            // 적의 공격 동작 수행
            enemy.Attack();
        }
    }
}
