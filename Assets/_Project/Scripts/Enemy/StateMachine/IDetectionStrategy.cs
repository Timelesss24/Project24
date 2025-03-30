using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Timelesss
{
    public interface IDetectionStrategy
    {
        /// <summary>
        /// ���� ���� �޼���:
        /// Detector(������)�� ���(Target)�� ������ �� �ִ��� ���θ� �Ǵ��մϴ�.
        /// </summary>
        /// <param name="target">���� ��� Ʈ������ (�ַ� �÷��̾�)</param>
        /// <param name="detector">������ Ʈ������</param>
        /// <param name="timer">
        /// ���� ������ Ÿ�̸ӷ� �����Ͽ� ���� ������ �����մϴ�.
        /// Ÿ�̸Ӱ� ���� ���� ��� ������ �̷������ �ʽ��ϴ�.
        /// </param>
        /// <returns>������ �����ϸ� true��, �����ϸ� false�� ��ȯ.</returns>
        bool Execute(Transform target, Transform detector, CountdownTimer timer);
    }

    /// <summary>
    /// ���� ����(Cone)�� ������ ������ ��ü���� ���� ���� Ŭ�����Դϴ�.
    /// Ư�� ������ ������ �������� ����� �����մϴ�.
    /// </summary>
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        // ���� ���� (Detection Angle): ���� ���濡�� ���� ������ ���� ����
        readonly float detectionAngle;

        // ���� �ݰ�(Radius): ���� ������ �� �ִ� �ִ� �Ÿ�
        readonly float detectionRadius;

        // ���� �ݰ�(Inner Radius): ���� �Ÿ�(�ʹ� ��������� ���) �ȿ����� ������ �Ұ����ϵ��� ����
        readonly float innerDetectionRadius;

        /// <summary>
        /// ConeDetectionStrategy ������:
        /// ���� ����, �ܺ� �ݰ�, ���� �ݰ��� �����մϴ�.
        /// </summary>
        /// <param name="detectionAngle">���� ����(�� ����)</param>
        /// <param name="detectionRadius">���� �ִ� �ݰ�</param>
        /// <param name="innerDetectionRadius">���� �ּ� �ݰ� (���� �ݰ�)</param>
        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
        {
            // ���� ������ �ݰ��� �ʱ�ȭ
            this.detectionAngle = detectionAngle;
            this.detectionRadius = detectionRadius;
            this.innerDetectionRadius = innerDetectionRadius;
        }

        /// <summary>
        /// ���� ���� �޼���:
        /// ������(Detector)�� ����� ������ �� �ִ��� �Ǵ��մϴ�.
        /// </summary>
        /// <param name="target">���� ��� Ʈ������ (�ַ� �÷��̾�)</param>
        /// <param name="detector">������ Ʈ������ (��)</param>
        /// <param name="timer">������ ���� Ÿ�̸� ��ü</param>
        /// <returns>���� ���� ����</returns>
        bool IDetectionStrategy.Execute(Transform target, Transform detector, CountdownTimer timer)
        {
            // Ÿ�̸Ӱ� �۵� ���̸� ���� ���� �Ұ�
            if (timer.IsRunning) return false;

            // �������� ���� ���
            var directionToTarget = target.position - detector.position;

            // ���� �������� ���� ������ ���� ���
            var angleToTarget = Vector3.Angle(directionToTarget, detector.forward);

            // ���� ����:
            // ����� ���� ���� �� �ܺ� �ݰ� ���� ������,
            // ���� �ݰ� ���� ���� ���� ��쿡�� ���� ����.
            if ((!(angleToTarget < detectionAngle / 2f) || !(directionToTarget.magnitude < detectionRadius))
                // ���� �ݰ� ���� �ִ� ��� ���� ����
                && !(directionToTarget.magnitude < innerDetectionRadius))
                return false;

            // ���� ����: Ÿ�̸� ���� �� true ��ȯ
            timer.Start();
            return true;
        }
    }
}
