using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Timelesss
{
    public class PlayerDetector : MonoBehaviour
    {
        // 감지 각도: 적의 전방에서 플레이어를 탐지할 수 있는 각도 범위 (단위: 도)
        //[SerializeField] float detectionAngle = 60f;

        // 감지 반경: 적이 플레이어를 탐지할 수 있는 최대 거리 (단위: 유닛)
        //[SerializeField] float detectionRadius = 10f;

        // 내부 감지 반경: 너무 가까운 거리에서는 탐지가 이루어지지 않도록 설정
        //[SerializeField] float innerDetectionRadius = 5f;

        // 감지 재사용 대기 시간: 감지 시 동작이 발생한 후 일정 시간 동안 재감지 제한
        //[SerializeField] float detectionCooldown = 1f;

        // 공격 범위: 적이 플레이어를 공격할 수 있는 거리
        //[SerializeField] float attackRange = 2f;

        //위의 변수를 전부 스크립터블 오브젝트로 관리
        [SerializeField] public EnemyOS Date;

        // 탐지할 대상(플레이어)의 Transform (수동 설정 없이 태그로 자동 연결)
        public Transform Target { get; private set; }

        // 탐지 대상(플레이어)의 Health 컴포넌트 (체력 관리)
        public PlayerInfo TargetInfo { get; private set; }

        // 감지 쿨타임을 구현한 카운트다운 타이머 객체
        CountdownTimer detectionTimer;

        // 실제 탐지 로직을 수행할 감지 전략 객체 (인터페이스 IDetectionStrategy 기반)
        IDetectionStrategy detectionStrategy;

        /// <summary>
        /// Awake: 스크립트 초기화 단계.
        /// 플레이어를 감지 대상으로 설정하고 Health 컴포넌트를 가져옵니다.
        /// </summary>
        void Awake()
        {
            // "Player" 태그를 가진 객체를 찾아 Target으로 설정
            Target = GameObject.FindGameObjectWithTag("Player").transform;

            // Target으로부터 Health 컴포넌트를 가져옵니다.
            TargetInfo = Target.GetComponent<PlayerInfo>();
        }

        /// <summary>
        /// Start: 게임 시작 시 실행.
        /// 타이머 객체 및 기본 탐지 전략(ConeDetectionStrategy)을 초기화합니다.
        /// </summary>
        void Start()
        {
            // Timer 객체 초기화 (감지 쿨타임 설정)
            detectionTimer = new CountdownTimer(Date.detectionCooldown);

            // 기본 탐지 전략으로 원뿔 형태의 탐지 전략을 설정
            detectionStrategy = new ConeDetectionStrategy(Date.detectionAngle, Date.detectionRadius, Date.innerDetectionRadius);
        }

        /// <summary>
        /// Update: 매 프레임마다 실행됩니다.
        /// 탐지 타이머를 갱신하여 감지 간격을 유지합니다.
        /// </summary>
        void Update() => detectionTimer.Tick(Time.deltaTime);

        /// <summary>
        /// 플레이어를 탐지할 수 있는지 확인합니다.
        /// 내부적으로 감지 전략 및 조건에 따라 탐지 여부를 반환합니다.
        /// </summary>
        /// <returns>플레이어를 탐지할 수 있으면 true, 그렇지 않으면 false.</returns>
        public bool CanDetectPlayer()
        {
            // 타이머가 실행 중이거나 탐지 전략의 조건이 충족되면 플레이어 감지
            return detectionTimer.IsRunning || detectionStrategy.Execute(Target, transform, detectionTimer);
        }

        /// <summary>
        /// 플레이어를 가까운 거리에서 공격할 수 있는지 확인합니다.
        /// </summary>
        /// <returns>플레이어가 공격 가능 범위에 있으면 true, 그렇지 않으면 false.</returns>
        public bool CanAttackPlayer()
        {
            // 대상과 자신의 거리 계산
            var direction = Target.position - transform.position;

            // 거리 확인 (공격 범위 내에 있는 경우 true 반환)
            return direction.magnitude <= Date.attackRange;
        }

        /// <summary>
        /// 탐지 전략을 교체합니다.
        /// IDetectionStrategy를 구현하는 새로운 감지 전략을 설정할 수 있습니다.
        /// </summary>
        /// <param name="strategy">새로운 감지 전략 객체</param>
        public void SetDetectionStrategy(IDetectionStrategy strategy) => detectionStrategy = strategy;

        /// <summary>
        /// 디버깅 정보를 표시하기 위해 Gizmos를 그립니다.
        /// 감지 반경 및 각도를 시각화하여 개발 시 확인 가능합니다.
        /// </summary>
        void OnDrawGizmos()
        {
            // Gizmos 색상 설정 (빨간색)
            Gizmos.color = Color.red;

            // 감지 반경 및 내부 반경을 나타내는 구를 그림
            Gizmos.DrawWireSphere(transform.position, Date.detectionRadius);
            Gizmos.DrawWireSphere(transform.position, Date.innerDetectionRadius);

            // 감지 각도를 계산
            var forwardConDirection = Quaternion.Euler(0, Date.detectionAngle / 2, 0) * transform.forward * Date.detectionRadius;
            var backwardConDirection = Quaternion.Euler(0, -Date.detectionAngle / 2, 0) * transform.forward * Date.detectionRadius;

            // 감지 각도 범위(원뿔 형태)를 시각화 (양쪽 선 그리기)
            Gizmos.DrawLine(transform.position, transform.position + forwardConDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConDirection);
        }
    }
}
