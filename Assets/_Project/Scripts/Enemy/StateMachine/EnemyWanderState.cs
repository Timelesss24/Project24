using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Timelesss
{
    public class EnemyWanderState : EnemyBaseState
    {
        // ���� �̵��� �����ϴ� NavMeshAgent ������Ʈ
        readonly NavMeshAgent agent;

        // ���� ���� ��ġ�� �����Ͽ� ��ȸ ������ �������� ����
        readonly Vector3 startPoint;

        // ��ȸ ������ �ݰ�
        float wanderRadius;

        /// <summary>
        /// EnemyWanderState ������:
        /// ��ȸ ���¿� ���õ� ��, �ִϸ�����, NavMeshAgent, ��ȸ �ݰ��� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="enemy">���� �� ��ü</param>
        /// <param name="animator">���� �ִϸ��̼� ������Ʈ</param>
        /// <param name="agent">���� �̵��� ��� Ž���� ����ϴ� NavMeshAgent</param>
        /// <param name="wanderRadius">���� ��ȸ�� �� �ִ� �ִ� �ݰ�</param>
        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius)
            : base(enemy, animator) // �⺻ ���� Ŭ������ ������ ȣ��
        {
            this.wanderRadius = wanderRadius;
            this.startPoint = agent.transform.position; // ��ȸ ���� ���� ����
            this.agent = agent;
        }

        /// <summary>
        /// ���°� Ȱ��ȭ(����)�� �� ����Ǵ� �Լ��Դϴ�.
        /// ���� "�ȱ�(Walk)" �ִϸ��̼��� ����մϴ�.
        /// </summary>
        public override void OnEnter()
        {
            // Walk �ִϸ��̼� ����
            animator.CrossFade(WalkHash, crossFadeDuration);
        }

        /// <summary>
        /// ���°� Ȱ��ȭ�Ǿ� �ִ� ���� �� ������ ȣ��˴ϴ�.
        /// ���� �������� ������ ���, ���ο� ��ȸ �������� �������� �����ϰ� �̵� ��θ� �����մϴ�.
        /// </summary>
        public override void Update()
        {
            // ���� ���� �������� �������� ���
            if (HasReachedDestination())
            {
                // �ݰ� ������ ������ ���� ���͸� ����
                var randomDirection = Random.insideUnitSphere * wanderRadius;

                // ���� ������ �������� ��ȸ ���� ����� �ʵ��� ����
                randomDirection += startPoint;

                // �ش� ������ NavMesh �󿡼� ��ȿ�� ��ġ���� Ȯ�� �� ����
                NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, 1);

                // ���� ������ ����
                var finalPosition = hit.position;

                // NavMeshAgent�� �̿��� ���� �������� �̵���Ŵ
                agent.SetDestination(finalPosition);
            }
        }

        /// <summary>
        /// ���°� ��Ȱ��ȭ(����)�� �� ����Ǵ� �Լ��Դϴ�.
        /// ��ȸ ������ ���߰� ��� �����͸� �ʱ�ȭ�մϴ�.
        /// </summary>
        public override void OnExit()
        {
            // NavMeshAgent�� ���� ��θ� �ʱ�ȭ�Ͽ� �̵��� ����
            agent.ResetPath();
        }

        /// <summary>
        /// ���� ���� ������ �������� �����ߴ��� Ȯ���մϴ�.
        /// ������ ���� ���δ� NavMeshAgent�� ���¿� ���� �Ÿ� ���� ������� �Ǵ��մϴ�.
        /// </summary>
        /// <returns>���� �������� �����ߴٸ� true, �׷��� ������ false.</returns>
        bool HasReachedDestination()
        {
            // NavMeshAgent�� ���¸� ������� ������ ���� ���� Ȯ��:
            return !agent.pathPending && // ���ο� ��� ������� �ƴϸ�
                   agent.remainingDistance <= agent.stoppingDistance && // ���� �Ÿ��� ���� �Ÿ����� �۰ų� ������
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f); // ��ΰ� ���ų� �ӵ��� 0�� ���
        }
    }
}
