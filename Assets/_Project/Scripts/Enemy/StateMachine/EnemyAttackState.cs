using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Timelesss
{
    public class EnemyAttackState : EnemyBaseState
    {
        // ���� �̵��� ��� Ž���� ����ϴ� NavMeshAgent ������Ʈ
        readonly NavMeshAgent agent;

        // ���� ����� �÷��̾��� Transform
        readonly Transform target;

        /// EnemyAttackState ������:
        /// �� ��ü, �ִϸ�����, NavMeshAgent, ���� Ÿ���� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="enemy">���� �� ��ü</param>
        /// <param name="animator">���� �ִϸ��̼��� �����ϴ� Animator</param>
        /// <param name="agent">���� �̵��� ��� Ž���� ����ϴ� NavMeshAgent</param>
        /// <param name="target">���� ������ ��� (�÷��̾��� Transform)</param>
        public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform target)
            : base(enemy, animator) // �θ� Ŭ������ ������ ȣ��
        {
            this.agent = agent; // NavMeshAgent �ʱ�ȭ
            this.target = target; // ���� ��� �ʱ�ȭ
        }

        /// ���°� Ȱ��ȭ(����)�� �� ����˴ϴ�.
        /// ���� ���� ���� ��, ���� �ִϸ��̼��� �����մϴ�.
        public override void OnEnter()
        {
            // Attack �ִϸ��̼��� �ε巴�� ���� (CrossFade ���)
            animator.CrossFade(AttackHash, crossFadeDuration);
        }

        /// <summary>
        /// ���°� Ȱ��ȭ�Ǿ� �ִ� ���� �� ������ ȣ��˴ϴ�.
        /// ���� ���� ����� �ٶ󺸸�, ���� ������ �����մϴ�.
        /// </summary>
        public override void Update()
        {
            // Ÿ���� ��ġ�� NavMeshAgent�� ���̿� �����ϰ� ������ ��ġ�� ����
            var targetPosition = new Vector3(target.position.x, agent.transform.position.y, target.position.z);

            // ��(NavMeshAgent)�� Ÿ���� �ٶ󺸵��� ����
            agent.transform.LookAt(targetPosition);

            // ���� ���� ���� ����
            enemy.Attack();
        }
    }
}
