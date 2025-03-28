using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Utilities;

namespace Timelesss
{
    public enum AttackStates
    {
        Idle,
        Windup,
        Impact,
        Cooldown
    }
    public class CombatController : MonoBehaviour
    {
        [SerializeField] WeaponData weapon;

        AnimationSystem animationSystem;

        public AttackStates AttackState { get; private set; }
        public Action OnStartAttack;
        public Action OnEndAttack;

        int comboCount;
        bool doCombo;
        CountdownTimer attackTimer;

        void Start()
        {
            var playerController = GetComponent<PlayerController>();
            animationSystem = playerController.AnimationSystem;

            weapon.InIt();

            attackTimer = new CountdownTimer(0f);

            OnStartAttack += attackTimer.Start;
            OnEndAttack += attackTimer.Stop;
        }

        void Update()
        {
            attackTimer.Tick(Time.deltaTime);
        }
        public void TryAttack()
        {
            if (!weapon) return;
            HandleAttack();
        }

        void HandleAttack()
        {
            if (AttackState == AttackStates.Idle)
            {
                StartCoroutine(Attack());
            }
            else if (AttackState is AttackStates.Impact or AttackStates.Cooldown)
            {
                Debug.Log("doCombo");
                doCombo = true;
            }
        }
        IEnumerator Attack()
        {
            AttackState = AttackStates.Windup;

            var attackList = weapon.AttacksContainer.Attacks;
            var attackSlot = attackList[comboCount];
            var attack = attackSlot.Attack;
            //todo ChargeAttack 추가

            var attackDir = transform.forward;
            var startPos = transform.position;
            var targetPos = Vector3.zero;
            var rootMotionScaleFactor = Vector3.one;


            animationSystem.PlayOneShot(attack.Clip);

            float attackLength = attack.Clip.length;
            attackTimer.Reset(attackLength);

            OnStartAttack?.Invoke();

            while (attackTimer.IsRunning)
            {
                float normalizedTime = attackTimer.Progress;
                //AttackTimeNormalized = normalizedTime;

                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(attackDir), 2f * Time.deltaTime);

                var hitPoint = Vector3.zero;
                if (AttackState == AttackStates.Windup)
                {
                    if (normalizedTime <= 1f- attack.ImpactStartTime)
                    {
                        AttackState = AttackStates.Impact;
                        //todo 무기 콜라이더 활성화
                    }
                }
                else if (AttackState == AttackStates.Impact)
                {

                    if (normalizedTime <= 1f- attack.ImpactEndTime)
                    {
                        AttackState = AttackStates.Cooldown;
                        //todo 무기 콜라이더 비활성화
                    }
                }
                else if (AttackState == AttackStates.Cooldown)
                {
                    if (doCombo && attackList.Count > 0)
                    {
                        Debug.Log("Combo");
                        // Play next attack from combo
                        doCombo = false;

                        comboCount = (comboCount + 1) % attackList.Count;

                        StartCoroutine(Attack());
                        yield break;
                    }
                }

                yield return null;
            }


            AttackState = AttackStates.Idle;
            comboCount = 0;

            OnEndAttack?.Invoke();
        }

        void OnGUI()
        {
            // 공격 상태를 디버깅하기 위한 간단한 UI 작성
            GUIStyle debugStyle = new GUIStyle
            {
                fontSize = 20,
                normal = { textColor = Color.white }
            };

            // 왼쪽 상단 구석에 현재 상태와 정보 표시
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label($"Attack State: {AttackState}", debugStyle);

            var attack = weapon.AttacksContainer.Attacks[comboCount].Attack;
            GUILayout.Label($"Attack Length: {attack.Clip.length}", debugStyle);
            GUILayout.Label($"Attack Progress: {attackTimer.Progress}", debugStyle);
            GUILayout.Label($"ImpactStart: {1f-attack.ImpactStartTime}", debugStyle);
            GUILayout.Label($"ImpactEnd: {1f-attack.ImpactEndTime}", debugStyle);
            GUILayout.Label($"Combo Count: {comboCount}", debugStyle);
            GUILayout.Label($"Do Combo: {doCombo}", debugStyle);
            GUILayout.EndArea();
        }
    }
}