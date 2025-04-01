using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Timelesss
{
    public class BossIdleState : BossBaseState
    {
        // 적의 이동을 수행하는 NavMeshAgent 컴포넌트
        readonly NavMeshAgent agent;

        // 적의 시작 위치를 저장하여 배회 범위를 기준으로 삼음
        readonly Vector3 startPoint;

        // 배회 가능한 반경
        float wanderRadius;

        /// <summary>
        /// EnemyWanderState 생성자:
        /// 배회 상태와 관련된 적, 애니메이터, NavMeshAgent, 배회 반경을 초기화합니다.
        /// </summary>
        /// <param name="enemy">현재 적 객체</param>
        /// <param name="animator">적의 애니메이션 컴포넌트</param>
        /// <param name="agent">적의 이동과 경로 탐색을 담당하는 NavMeshAgent</param>
        /// <param name="wanderRadius">적이 배회할 수 있는 최대 반경</param>
        public BossIdleState(BossEnemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius)
            : base(enemy, animator) // 기본 상태 클래스의 생성자 호출
        {
            this.wanderRadius = wanderRadius;
            this.startPoint = agent.transform.position; // 배회 시작 지점 설정
            this.agent = agent;
        }

        /// <summary>
        /// 상태가 활성화(진입)될 때 실행되는 함수입니다.
        /// 적의 "걷기(Walk)" 애니메이션을 재생합니다.
        /// </summary>
        public override void OnEnter()
        {
            // Walk 애니메이션 실행
            animator.CrossFade(WalkHash, crossFadeDuration);
            Debug.Log("방황");
        }

        /// <summary>
        /// 상태가 활성화되어 있는 동안 매 프레임 호출됩니다.
        /// 적이 목적지에 도달한 경우, 새로운 배회 목적지를 무작위로 설정하고 이동 경로를 갱신합니다.
        /// </summary>
        public override void Update()
        {
            // 적이 이전 목적지에 도달했을 경우
            if (HasReachedDestination())
            {
                // 반경 내에서 무작위 방향 벡터를 생성
                var randomDirection = Random.insideUnitSphere * wanderRadius;

                // 시작 지점을 기준으로 배회 영역 벗어나지 않도록 보정
                randomDirection += startPoint;

                // 해당 방향이 NavMesh 상에서 유효한 위치인지 확인 및 보정
                NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, 1);

                // 최종 목적지 설정
                var finalPosition = hit.position;

                // NavMeshAgent를 이용해 적을 목적지로 이동시킴
                agent.SetDestination(finalPosition);
            }
        }

        /// <summary>
        /// 상태가 비활성화(종료)될 때 실행되는 함수입니다.
        /// 배회 동작을 멈추고 경로 데이터를 초기화합니다.
        /// </summary>
        public override void OnExit()
        {
            // NavMeshAgent의 현재 경로를 초기화하여 이동을 멈춤
            agent.ResetPath();
        }

        /// <summary>
        /// 적이 현재 지정된 목적지에 도달했는지 확인합니다.
        /// 목적지 도달 여부는 NavMeshAgent의 상태와 남은 거리 등을 기반으로 판단합니다.
        /// </summary>
        /// <returns>적이 목적지에 도달했다면 true, 그렇지 않으면 false.</returns>
        bool HasReachedDestination()
        {
            // NavMeshAgent의 상태를 기반으로 목적지 도달 여부 확인:
            return !agent.pathPending && // 새로운 경로 계산중이 아니며
                   agent.remainingDistance <= agent.stoppingDistance && // 남은 거리가 정지 거리보다 작거나 같으며
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f); // 경로가 없거나 속도가 0일 경우
        }
    }
}
