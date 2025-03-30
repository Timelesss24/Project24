using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.AI;
using Utilities;

namespace Timelesss
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        //[SerializeField] public EnemyOS Date;
        // NavMeshAgent ��ü: ���� ��� Ž�� �� �̵� ó���� ���
        [SerializeField, Self] NavMeshAgent agent;

        // PlayerDetector ��ü: �÷��̾� Ž���� ���� ������ ����
        [SerializeField, Self] PlayerDetector playerDetector;

        // Animator ��ü: ���� �ִϸ��̼� ���¸� ����
        [SerializeField, Child] Animator animator;

        // ���� ������ �̵� �ݰ� (wander radius)�� ����
        //[SerializeField] float wanderRadius = 10f;

        // ���� ��� �ð� (�ð� ���� ����)
        //[SerializeField] float timeBetweenAttacks = 1f; // ���� ���� ��ũ��Ʈ�� ��ü ���ɼ� ����

        // ���� ���(State Machine) ��ü: ���� ���� ��ȯ �� ���� ����
        StateMachine stateMachine;

        // ī��Ʈ�ٿ� Ÿ�̸� ��ü: ���� �� ������ ����
        CountdownTimer attackTimer;

        /// Unity�� OnValidate �޼���:
        /// �����Ϳ��� ������Ʈ�� �����Ǿ����� ��ȿ�� �˻� �� �ڵ� ����.
        /// </summary>
        void OnValidate() => this.ValidateRefs();

        /// Unity�� Start �޼���:
        /// �� Ŭ������ �ʱ�ȭ�� �����ϸ�, ���� ���� ���� Ÿ�̸� ������ ó���մϴ�.
         
        
        void Start()
        {
            // ���� Ÿ�̸� �ʱ�ȭ
            attackTimer = new CountdownTimer(playerDetector.Date.timeBetweenAttacks);

            // ���� ��� �ʱ�ȭ
            stateMachine = new StateMachine();

            // ���� ���� (�� AI�� �ֿ� ���۵� ����)
            var wanderState = new EnemyWanderState(this, animator, agent, playerDetector.Date.wanderRadius); // ������ ��Ȳ ����
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Target); // �÷��̾� ���� ����
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Target); // �÷��̾� ���� ����

            // ���� ��ȯ ���� ����
            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer())); // �÷��̾� Ž�� �� ����
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer())); // Ž�� ���� �� ��Ȳ
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer())); // ���� ���� �� ���� ���� ����
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer())); // ���� �Ұ� �� ����

            // �ʱ� ���� ���� (��Ȳ ���·� ����)
            stateMachine.SetState(wanderState);
        }

        /// <summary>
        /// �� ���� ���� ��ȯ ������ �߰��ϴ� ���� �޼���.
        /// </summary>
        /// <param name="from">���� ����</param>
        /// <param name="to">���� ����</param>
        /// <param name="condition">��ȯ ����</param>
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// � ���¿����� Ư�� ���·� ��ȯ�� �����ϵ��� �����ϴ� �޼���.
        /// </summary>
        /// <param name="to">���� ����</param>
        /// <param name="conditions">��ȯ ����</param>
        void Any(IState to, IPredicate conditions) => stateMachine.AddAnyTransition(to, conditions);

        /// <summary>
        /// Unity�� Update �޼���:
        /// �� �����Ӹ��� ���� ��� ������Ʈ �� ���� Ÿ�̸� ����
        /// </summary>
        void Update()
        {
            // ���� ����� ���� ���� ������Ʈ
            stateMachine.Update();

            // ���� Ÿ�̸� �ð� ���
            attackTimer.Tick(Time.deltaTime);
        }

        /// <summary>
        /// Unity�� FixedUpdate �޼���:
        /// �������� ���� �� ���� ������Ʈ�� ������ ������ ó��
        /// </summary>
        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// ���� ������ ó���ϴ� �޼���. 
        /// Ÿ�̸Ӱ� ���� ���� ��� ������ �������� ����.
        /// </summary>
        public void Attack()
        {
            if (attackTimer.IsRunning) return; // Ÿ�̸ӿ� ���� ���� �� ���� ����

            // ���� ����
            attackTimer.Start();
            //playerDetector.TargetHealth.TakeDamage(10); // �÷��̾�� ���� (�ӽ÷� 10 ����)
        }
        void OnHit()
        {
            animator.SetTrigger("Hit");
            OnDie();
        }
        void OnDie()
        {
            animator.SetTrigger("Die");
        }
    }
}
