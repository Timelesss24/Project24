using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Timelesss
{
    public interface IDetectionStrategy
    {
        /// <summary>
        /// 감지 실행 메서드:
        /// Detector(감지자)가 대상(Target)을 감지할 수 있는지 여부를 판단합니다.
        /// </summary>
        /// <param name="target">감지 대상 트랜스폼 (주로 플레이어)</param>
        /// <param name="detector">감지자 트랜스폼</param>
        /// <param name="timer">
        /// 감지 간격을 타이머로 관리하여 연속 감지를 제한합니다.
        /// 타이머가 실행 중인 경우 감지는 이루어지지 않습니다.
        /// </param>
        /// <returns>감지에 성공하면 true를, 실패하면 false를 반환.</returns>
        bool Execute(Transform target, Transform detector, CountdownTimer timer);
    }

    /// <summary>
    /// 원뿔 형태(Cone)로 감지를 구현한 구체적인 감지 전략 클래스입니다.
    /// 특정 각도와 범위를 기준으로 대상을 감지합니다.
    /// </summary>
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        // 감지 각도 (Detection Angle): 적의 전방에서 감지 가능한 각도 범위
        readonly float detectionAngle;

        // 감지 반경(Radius): 적이 감지할 수 있는 최대 거리
        readonly float detectionRadius;

        // 내부 반경(Inner Radius): 일정 거리(너무 가까워지는 경우) 안에서는 감지가 불가능하도록 제한
        readonly float innerDetectionRadius;

        /// <summary>
        /// ConeDetectionStrategy 생성자:
        /// 감지 각도, 외부 반경, 내부 반경을 설정합니다.
        /// </summary>
        /// <param name="detectionAngle">감지 각도(도 단위)</param>
        /// <param name="detectionRadius">감지 최대 반경</param>
        /// <param name="innerDetectionRadius">감지 최소 반경 (내부 반경)</param>
        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
        {
            // 감지 각도와 반경을 초기화
            this.detectionAngle = detectionAngle;
            this.detectionRadius = detectionRadius;
            this.innerDetectionRadius = innerDetectionRadius;
        }

        /// <summary>
        /// 감지 수행 메서드:
        /// 감지자(Detector)가 대상을 감지할 수 있는지 판단합니다.
        /// </summary>
        /// <param name="target">감지 대상 트랜스폼 (주로 플레이어)</param>
        /// <param name="detector">감지자 트랜스폼 (적)</param>
        /// <param name="timer">감지를 위한 타이머 객체</param>
        /// <returns>감지 성공 여부</returns>
        bool IDetectionStrategy.Execute(Transform target, Transform detector, CountdownTimer timer)
        {
            // 타이머가 작동 중이면 감지 수행 불가
            if (timer.IsRunning) return false;

            // 대상까지의 벡터 계산
            var directionToTarget = target.position - detector.position;

            // 대상과 감지자의 전방 사이의 각도 계산
            var angleToTarget = Vector3.Angle(directionToTarget, detector.forward);

            // 감지 로직:
            // 대상이 감지 각도 및 외부 반경 내에 있으며,
            // 내부 반경 내에 있지 않은 경우에만 감지 성공.
            if ((!(angleToTarget < detectionAngle / 2f) || !(directionToTarget.magnitude < detectionRadius))
                // 내부 반경 내에 있는 경우 감지 실패
                && !(directionToTarget.magnitude < innerDetectionRadius))
                return false;

            // 감지 성공: 타이머 시작 및 true 반환
            timer.Start();
            return true;
        }
    }
}
