using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.AI;
using Utilities;
using UnityEditor;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;

namespace Timelesss
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        //[SerializeField] public EnemyOS Date;
        // NavMeshAgent 객체: 적의 경로 탐색 및 이동 처리를 담당
        [SerializeField, Self] NavMeshAgent agent;

        // PlayerDetector 객체: 플레이어 탐지와 공격 범위를 관리
        [SerializeField, Self] PlayerDetector playerDetector;

        // Animator 객체: 적의 애니메이션 상태를 관리
        [SerializeField, Child] Animator animator;

        // 적의 무작위 이동 반경 (wander radius)을 정의
        //[SerializeField] float wanderRadius = 10f;

        // 공격 대기 시간 (시간 간격 설정)
        //[SerializeField] float timeBetweenAttacks = 1f; // 추후 무기 스크립트로 대체 가능성 있음

        // 상태 기계(State Machine) 객체: 적의 상태 전환 및 동작 관리
        StateMachine stateMachine;

        // 카운트다운 타이머 객체: 공격 간 간격을 추적
        CountdownTimer attackTimer;

        /// Unity의 OnValidate 메서드:
        /// 에디터에서 컴포넌트가 설정되었는지 유효성 검사 및 자동 설정.
        //[SerializeField] SkinnedMeshRenderer skinRenderer;
        public Image hpBar;
        private float enemyHp;
        private Transform enemyTransform;
        public bool isDie = false;

        public event System.Action OnDamageTaken;

        public EnemyOSEventChannel enemyEventChannel;

        void OnValidate() => this.ValidateRefs();

        /// Unity의 Start 메서드:
        /// 적 클래스의 초기화를 수행하며, 상태 기계와 공격 타이머 설정을 처리합니다.
        void Start()
        {
            // 공격 타이머 초기화
            attackTimer = new CountdownTimer(playerDetector.Date.timeBetweenAttacks);

            // 상태 기계 초기화
            stateMachine = new StateMachine();

            // 상태 정의 (적 AI의 주요 동작들 구현)
            var wanderState = new EnemyWanderState(this, animator, agent, playerDetector.Date.wanderRadius); // 무작위 방황 상태
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Target); // 플레이어 추적 상태
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Target); // 플레이어 공격 상태

            // 상태 전환 조건 정의
            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer())); // 플레이어 탐지 시 공격
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer())); // 탐지 실패 시 방황
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer())); // 공격 가능 시 공격 상태 진입
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer())); // 공격 불가 시 추적

            //skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            enemyTransform = GetComponent<Transform>();
            hpBar = GetComponentInChildren<Image>();
            enemyHp = playerDetector.Date.maxHp;
            // 초기 상태 설정 (방황 상태로 시작)
            stateMachine.SetState(wanderState);
        }

        /// <summary>
        /// 두 상태 간의 전환 조건을 추가하는 헬퍼 메서드.
        /// </summary>
        /// <param name="from">이전 상태</param>
        /// <param name="to">다음 상태</param>
        /// <param name="condition">전환 조건</param>
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// 어떤 상태에서든 특정 상태로 전환이 가능하도록 설정하는 메서드.
        /// </summary>
        /// <param name="to">다음 상태</param>
        /// <param name="conditions">전환 조건</param>
        void Any(IState to, IPredicate conditions) => stateMachine.AddAnyTransition(to, conditions);

        /// <summary>
        /// Unity의 Update 메서드:
        /// 매 프레임마다 상태 기계 업데이트 및 공격 타이머 갱신
        /// </summary>
        void Update()
        {
            // 상태 기계의 현재 상태 업데이트
            if (isDie != true)
            { stateMachine.Update(); }

            // 공격 타이머 시간 계산
            attackTimer.Tick(Time.deltaTime);
        }

        /// <summary>
        /// Unity의 FixedUpdate 메서드:
        /// 물리적인 동작 및 상태 업데이트를 프레임 단위로 처리
        /// </summary>
        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// 공격 동작을 처리하는 메서드. 
        /// 타이머가 실행 중인 경우 공격을 실행하지 않음.
        /// </summary>
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

            enemyEventChannel?.Invoke(playerDetector.Date);
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
            if (hpBar != null)
                hpBar.fillAmount = enemyHp / playerDetector.Date.maxHp;
        }
        void FadeOutDestroy()
        {

            //if (skinRenderer != null)//투명화. skinnd mesh randerer에서 안 먹힘
            //{
            //    foreach (Material mat in skinRenderer.materials)
            //    {
            //        Debug.Log("죽음");

            //        mat.DOFade(0f, 3.8f).SetEase(Ease.InOutSine).OnComplete(() => Destroy(this.gameObject));
            //    }
            //}

            if (enemyTransform != null)
            {
                enemyTransform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 2f).SetEase(Ease.InOutSine).OnComplete(() => DungeonManager.Instance.RemoveEnemy(gameObject));
            }
        }
        private IEnumerator DelayDie(float count)
        {
            yield return new WaitForSeconds(count);
            FadeOutDestroy();
        }
    }

    [CustomEditor(typeof(Enemy))]
    public class EnemyDie : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("1000 데미지"))
                {
                    ((Enemy)target).TakeDamage(1000);
                }
            }
        }
    }
}
