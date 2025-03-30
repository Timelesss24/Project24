using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public abstract class EnemyBaseState : IState
    {
        // ���� ���¿��� ������ ��(Enemy) ��ü
        protected readonly Enemy enemy;

        // ���� �ִϸ��̼��� ������ Animator ������Ʈ
        protected readonly Animator animator;

        // �ִϸ��̼� ���� �ؽ� (���� ����ȭ�� ���� Hash �� ���)
        protected static readonly int IdleHash = Animator.StringToHash("IdleNormal"); // Idle ����
        protected static readonly int RunHash = Animator.StringToHash("RunFWD"); // �޸��� ����
        protected static readonly int WalkHash = Animator.StringToHash("WalkFWD"); // �ȱ� ����
        protected static readonly int AttackHash = Animator.StringToHash("Attack01"); // ���� ����
        protected static readonly int DieHash = Animator.StringToHash("Die"); // ��� ����

        // �ִϸ��̼� ��ȯ �� �ڿ������� ��ȯ�� ���� CrossFade ���� �ð�
        protected const float crossFadeDuration = 0.1f;

        /// <summary>
        /// EnemyBaseState ������:
        /// ���� ���õ� ���� �����͸� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="enemy">���� �� ��ü</param>
        /// <param name="animator">���� �ִϸ��̼��� ����ϴ� Animator ������Ʈ</param>
        protected EnemyBaseState(Enemy enemy, Animator animator)
        {
            this.enemy = enemy;
            this.animator = animator;
        }

        /// <summary>
        /// ���°� Ȱ��ȭ(����)�� �� ����Ǵ� �޼���.
        /// ���� ��ȭ �� �⺻���� �ʱ� ������ �����մϴ�.
        /// </summary>
        public virtual void OnEnter()
        {
            // ���� ���� �� ����� �޽����� ����Ͽ� ���� ���¸� Ȯ��
            Debug.Log("Entering state: " + this.GetType().Name);
        }

        /// <summary>
        /// ���°� Ȱ��ȭ�Ǿ� �ִ� ���� �� ������ ȣ��˴ϴ�.
        /// ���� ���� �� �ʿ��� ������ ������ �� �ֽ��ϴ�.
        /// </summary>
        public virtual void Update()
        {
            // �⺻ ����: �������� ���� (�� ���¿��� �������̵� ����)
        }

        /// <summary>
        /// FixedUpdate: �������� ������ �ʿ��� ��� ȣ��˴ϴ�.
        /// ���� ���� �� �ʿ��� ���� ��� �ൿ ������ ������ �� �ֽ��ϴ�.
        /// </summary>
        public virtual void FixedUpdate()
        {
            // �⺻ ����: �������� ���� (�� ���¿��� �������̵� ����)
        }

        /// <summary>
        /// ���°� ��Ȱ��ȭ(����)�� �� ����Ǵ� �޼���.
        /// ���� ��ȯ �� �ʿ��� ���� �۾��� ������ �� �ֽ��ϴ�.
        /// </summary>
        public virtual void OnExit()
        {
            // �⺻ ����: �������� ���� (�� ���¿��� �������̵� ����)
        }
    }
}
