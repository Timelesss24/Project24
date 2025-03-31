using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Timelesss
{
    public class EnemyChaseState : EnemyBaseState
    {
        // ���� ������ �� ����ϴ� NavMeshAgent ������Ʈ
        readonly NavMeshAgent agent;

        // ���� ����� �÷��̾��� Transform
        readonly Transform target;

        /// <summary>
        /// EnemyChaseState�� ������.
        /// Ư�� �� ��ü�� �̸� ������ Animator, NavMeshAgent, ���� ���(Target)�� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="enemy">���� �� ��ü</param>
        /// <param name="animator">���� �ִϸ��̼��� �����ϴ� Animator</param>
        /// <param name="agent">���� �̵� �� ��� Ž���� ����ϴ� NavMeshAgent</param>
        /// <param name="target">�÷��̾��� Transform</param>
        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform target)
            : base(enemy, animator) // �⺻ ���� Ŭ������ ������ ȣ��
        {
            this.agent = agent;
            this.target = target;
        }

        /// <summary>
        /// ���°� Ȱ��ȭ(����)�� �� ����Ǵ� �Լ��Դϴ�.
        /// ���� ���¿� �ʿ��� �ʱ� ������ �����մϴ�.
        /// </summary>
        public override void OnEnter()
        {
            // Run �ִϸ��̼� ���� (CrossFade: �ڿ������� �ִϸ��̼� ��ȯ)
            animator.CrossFade(RunHash, crossFadeDuration);
        }

        /// <summary>
        /// ���°� Ȱ��ȭ�Ǿ� �ִ� ���� �� ������ ȣ��˴ϴ�.
        /// ����� ��ġ�� NavMeshAgent�� �����Ͽ� ���� �����ϵ��� �����մϴ�.
        /// </summary>
        public override void Update()
        {
            // Ÿ���� ���� ��ġ�� NavMeshAgent�� �̵��ϵ��� ����
            agent.SetDestination(target.position);
        }

        /// <summary>
        /// ���°� ��Ȱ��ȭ(����)�� �� ����Ǵ� �Լ��Դϴ�.
        /// ���� ������ ���߰� ��� ��θ� �ʱ�ȭ�մϴ�.
        /// </summary>
        public override void OnExit()
        {
            // ������ ���߰� NavMeshAgent�� ���� ��θ� �ʱ�ȭ
            agent.ResetPath();
        }
    }
}
