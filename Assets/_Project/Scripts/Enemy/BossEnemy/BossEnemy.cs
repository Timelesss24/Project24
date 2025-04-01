using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using UnityEngine.UI;
using DG.Tweening;

namespace Timelesss
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossEnemy : MonoBehaviour, IDamageable
    {
        [SerializeField, Self] NavMeshAgent agent;

        // PlayerDetector 객체: 플레이어 탐지와 공격 범위를 관리
        [SerializeField, Self] PlayerDetector playerDetector;
        
        // Animator 객체: 적의 애니메이션 상태를 관리
        [SerializeField, Child] Animator animator;

        // 상태 기계(State Machine) 객체: 적의 상태 전환 및 동작 관리
        StateMachine stateMachine;

        // 카운트다운 타이머 객체: 공격 간 간격을 추적
        CountdownTimer attackTimer;

        public Image hpBar;
        private float enemyHp;
        private Transform enemyTransform;
        public bool isDie = false;

        public event System.Action OnDamageTaken;

        void Start()
        {
            // 공격 타이머 초기화
            attackTimer = new CountdownTimer(playerDetector.Date.timeBetweenAttacks);

            // 상태 기계 초기화
            stateMachine = new StateMachine();

            // 상태 정의 (적 AI의 주요 동작들 구현)
            var idleState = new BossIdleState(this, animator, agent, playerDetector.Date.wanderRadius); // 무작위 방황 상태
            var walkState = new BossWalkState(this, animator, agent, playerDetector.Target); // 플레이어 추적 상태
            var attackState = new BossAttackState(this, animator, agent, playerDetector.Target); // 플레이어 공격 상태

            // 상태 전환 조건 정의
            At(idleState, walkState, new FuncPredicate(() => playerDetector.CanDetectPlayer())); // 플레이어 탐지 시 공격
            At(walkState, idleState, new FuncPredicate(() => !playerDetector.CanDetectPlayer())); // 탐지 실패 시 방황
            At(walkState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer())); // 공격 가능 시 공격 상태 진입
            At(attackState, walkState, new FuncPredicate(() => !playerDetector.CanAttackPlayer())); // 공격 불가 시 추적

            //skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            enemyTransform = GetComponent<Transform>();
            hpBar = GetComponentInChildren<Image>();
            enemyHp = playerDetector.Date.maxHp;
            // 초기 상태 설정 (방황 상태로 시작)
            stateMachine.SetState(idleState);
        }
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate conditions) => stateMachine.AddAnyTransition(to, conditions);
        void Update()
        {
            // 상태 기계의 현재 상태 업데이트
            if (isDie != true)
            { stateMachine.Update(); }

            // 공격 타이머 시간 계산
            attackTimer.Tick(Time.deltaTime);
        }
        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
        public void Attack()
        {
            if (isDie == true) return;
            if (attackTimer.IsRunning) return; // 타이머에 따라 공격 간 간격 유지

            // 공격 실행
            attackTimer.Start();
            playerDetector.TargetInfo.TakeDamage(playerDetector.Date.attackDamage); // 플레이어에게 피해
        }
        void OnHit()
        {
            if (isDie == true) return;
            animator.SetTrigger("Hit");

            if (enemyHp <= 0)
            {
                OnDie();
            }
        }
        void OnDie()
        {
            isDie = true;
            animator.SetTrigger("Die");

            playerDetector.TargetInfo.IncreasedExp(playerDetector.Date.exp);
            StartCoroutine(DelayDie(1.34f));
        }

        public void TakeDamage(int value)
        {
            int min = (int)(value * 0.8);
            int max = (int)(value * 1.2);
            int damage = Random.Range(min, max + 1);

            enemyHp -= damage;
            UpdateUI();
            Debug.Log($"적이 {damage}의 피해를 받았다!");

            OnHit();
        }
        void UpdateUI()
        {
            if(hpBar != null)
            hpBar.fillAmount = enemyHp / playerDetector.Date.maxHp;
        }
        void FadeOutDestroy()
        {
            if (enemyTransform != null)
            {
                enemyTransform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 2f).SetEase(Ease.InOutSine).OnComplete(() => Destroy(this.gameObject));
            }
        }
        private IEnumerator DelayDie(float count)
        {
            yield return new WaitForSeconds(count);
            FadeOutDestroy();
        }
    }
}
