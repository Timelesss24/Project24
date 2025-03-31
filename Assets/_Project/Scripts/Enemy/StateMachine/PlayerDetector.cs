using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Timelesss
{
    public class PlayerDetector : MonoBehaviour
    {
        // ���� ����: ���� ���濡�� �÷��̾ Ž���� �� �ִ� ���� ���� (����: ��)
        //[SerializeField] float detectionAngle = 60f;

        // ���� �ݰ�: ���� �÷��̾ Ž���� �� �ִ� �ִ� �Ÿ� (����: ����)
        //[SerializeField] float detectionRadius = 10f;

        // ���� ���� �ݰ�: �ʹ� ����� �Ÿ������� Ž���� �̷������ �ʵ��� ����
        //[SerializeField] float innerDetectionRadius = 5f;

        // ���� ���� ��� �ð�: ���� �� ������ �߻��� �� ���� �ð� ���� �簨�� ����
        //[SerializeField] float detectionCooldown = 1f;

        // ���� ����: ���� �÷��̾ ������ �� �ִ� �Ÿ�
        //[SerializeField] float attackRange = 2f;

        //���� ������ ���� ��ũ���ͺ� ������Ʈ�� ����
        [SerializeField] public EnemyOS Date;

        // Ž���� ���(�÷��̾�)�� Transform (���� ���� ���� �±׷� �ڵ� ����)
        public Transform Target { get; private set; }

        // Ž�� ���(�÷��̾�)�� Health ������Ʈ (ü�� ����)
        public PlayerInfo TargetInfo { get; private set; }

        // ���� ��Ÿ���� ������ ī��Ʈ�ٿ� Ÿ�̸� ��ü
        CountdownTimer detectionTimer;

        // ���� Ž�� ������ ������ ���� ���� ��ü (�������̽� IDetectionStrategy ���)
        IDetectionStrategy detectionStrategy;

        /// <summary>
        /// Awake: ��ũ��Ʈ �ʱ�ȭ �ܰ�.
        /// �÷��̾ ���� ������� �����ϰ� Health ������Ʈ�� �����ɴϴ�.
        /// </summary>
        void Awake()
        {
            // "Player" �±׸� ���� ��ü�� ã�� Target���� ����
            Target = GameObject.FindGameObjectWithTag("Player").transform;

            // Target���κ��� Health ������Ʈ�� �����ɴϴ�.
            TargetInfo = Target.GetComponent<PlayerInfo>();
        }

        /// <summary>
        /// Start: ���� ���� �� ����.
        /// Ÿ�̸� ��ü �� �⺻ Ž�� ����(ConeDetectionStrategy)�� �ʱ�ȭ�մϴ�.
        /// </summary>
        void Start()
        {
            // Timer ��ü �ʱ�ȭ (���� ��Ÿ�� ����)
            detectionTimer = new CountdownTimer(Date.detectionCooldown);

            // �⺻ Ž�� �������� ���� ������ Ž�� ������ ����
            detectionStrategy = new ConeDetectionStrategy(Date.detectionAngle, Date.detectionRadius, Date.innerDetectionRadius);
        }

        /// <summary>
        /// Update: �� �����Ӹ��� ����˴ϴ�.
        /// Ž�� Ÿ�̸Ӹ� �����Ͽ� ���� ������ �����մϴ�.
        /// </summary>
        void Update() => detectionTimer.Tick(Time.deltaTime);

        /// <summary>
        /// �÷��̾ Ž���� �� �ִ��� Ȯ���մϴ�.
        /// ���������� ���� ���� �� ���ǿ� ���� Ž�� ���θ� ��ȯ�մϴ�.
        /// </summary>
        /// <returns>�÷��̾ Ž���� �� ������ true, �׷��� ������ false.</returns>
        public bool CanDetectPlayer()
        {
            // Ÿ�̸Ӱ� ���� ���̰ų� Ž�� ������ ������ �����Ǹ� �÷��̾� ����
            return detectionTimer.IsRunning || detectionStrategy.Execute(Target, transform, detectionTimer);
        }

        /// <summary>
        /// �÷��̾ ����� �Ÿ����� ������ �� �ִ��� Ȯ���մϴ�.
        /// </summary>
        /// <returns>�÷��̾ ���� ���� ������ ������ true, �׷��� ������ false.</returns>
        public bool CanAttackPlayer()
        {
            // ���� �ڽ��� �Ÿ� ���
            var direction = Target.position - transform.position;

            // �Ÿ� Ȯ�� (���� ���� ���� �ִ� ��� true ��ȯ)
            return direction.magnitude <= Date.attackRange;
        }

        /// <summary>
        /// Ž�� ������ ��ü�մϴ�.
        /// IDetectionStrategy�� �����ϴ� ���ο� ���� ������ ������ �� �ֽ��ϴ�.
        /// </summary>
        /// <param name="strategy">���ο� ���� ���� ��ü</param>
        public void SetDetectionStrategy(IDetectionStrategy strategy) => detectionStrategy = strategy;

        /// <summary>
        /// ����� ������ ǥ���ϱ� ���� Gizmos�� �׸��ϴ�.
        /// ���� �ݰ� �� ������ �ð�ȭ�Ͽ� ���� �� Ȯ�� �����մϴ�.
        /// </summary>
        void OnDrawGizmos()
        {
            // Gizmos ���� ���� (������)
            Gizmos.color = Color.red;

            // ���� �ݰ� �� ���� �ݰ��� ��Ÿ���� ���� �׸�
            Gizmos.DrawWireSphere(transform.position, Date.detectionRadius);
            Gizmos.DrawWireSphere(transform.position, Date.innerDetectionRadius);

            // ���� ������ ���
            var forwardConDirection = Quaternion.Euler(0, Date.detectionAngle / 2, 0) * transform.forward * Date.detectionRadius;
            var backwardConDirection = Quaternion.Euler(0, -Date.detectionAngle / 2, 0) * transform.forward * Date.detectionRadius;

            // ���� ���� ����(���� ����)�� �ð�ȭ (���� �� �׸���)
            Gizmos.DrawLine(transform.position, transform.position + forwardConDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConDirection);
        }
    }
}
