using System;
using System.Collections;
using Core;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;
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
        [Header("References")]
        [SerializeField, Self] PlayerController playerController;
        [SerializeField, Self] AnimationSystem animationSystem;
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] PlayerInfo playerInfo;
        [FormerlySerializedAs("defaultData")]
        [FormerlySerializedAs("weaponData")]
        [FormerlySerializedAs("weapon")]
        [SerializeField, Anywhere] WeaponData defaultWeaponData;

        [SerializeField] LayerMask hitboxLayer;


        AttachedWeapon currentWeaponHandler;
        WeaponData currentWeaponData;
        GameObject currentWeaponObject;
        GameObject prevGameObj; // Collider sweep 공격 히트 체크용 
        BoxCollider weaponCollider;
        BoxCollider activeCollider;
        Vector3 prevColliderPos;

        CountdownTimer attackTimer;
        int comboCount;
        bool doCombo;

        public AttackStates AttackState { get; private set; }

        public event Action OnStartAttack;
        public event Action OnEndAttack;
        public event Action<AttachedWeapon> OnEnableHit;

        public bool IsAttacking => AttackState != AttackStates.Idle;
        public AttackData CurrentAttack { get; private set; }

        void OnValidate() => this.ValidateRefs();


        void Start()
        {
            animationSystem = playerController.AnimationSystem;

            attackTimer = new CountdownTimer(0f);

            OnStartAttack += attackTimer.Start;
            OnEndAttack += attackTimer.Stop;

            EquipWeapon(defaultWeaponData); // 기본 무기 장착
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (currentWeaponObject)
                    UnEquipWeapon();
                else
                    EquipWeapon(defaultWeaponData);

            }

            attackTimer.Tick(Time.deltaTime);
        }
        public void TryAttack()
        {
            if (!currentWeaponData) return;
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
                doCombo = true;
            }
        }
        IEnumerator Attack()
        {
            if (!playerInfo.UseStamina(10f))
            {
                AttackState = AttackStates.Idle;
                yield break;
            }

            AttackState = AttackStates.Windup;

            var attackList = currentWeaponData.AttacksContainer.Attacks;
            var attackSlot = attackList[comboCount];
            var attack = attackSlot.Attack;
            CurrentAttack = attack;
            //todo ChargeAttack 추가

            var attackDir = playerController.Movement == Vector3.zero ? transform.forward : playerController.CalculateTargetDirection();


            animationSystem.PlayOneShot(attack.Clip);

            float attackLength = attack.Clip.length;
            attackTimer.Reset(attackLength);

            OnStartAttack?.Invoke();

            while (attackTimer.IsRunning)
            {
                float normalizedTime = attackTimer.Progress;
                //AttackTimeNormalized = normalizedTime;

                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(attackDir), 100f * Time.deltaTime);

                if (AttackState == AttackStates.Windup)
                {
                    if (normalizedTime <= 1f - attack.ImpactStartTime)
                    {
                        AttackState = AttackStates.Impact;
                        EnableActiveCollider();
                        prevGameObj = null;
                    }
                }
                else if (AttackState == AttackStates.Impact)
                {
                    HandleColliderSweep();
                    if (normalizedTime <= 1f - attack.ImpactEndTime)
                    {
                        AttackState = AttackStates.Cooldown;
                        DisableActiveCollider();
                    }
                }
                else if (AttackState == AttackStates.Cooldown)
                {
                    if (doCombo && attackList.Count > 0)
                    {
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
        void HandleColliderSweep()
        {
            // 현재 활성화된 BoxCollider를 저장
            var activeBoxCollider = activeCollider;

            // Collider가 활성화되어 있는 경우에 처리를 진행
            if (activeBoxCollider)
            {
                // BoxCollider의 World 공간에서의 중심 좌표를 계산
                var endPoint = activeBoxCollider.transform.TransformPoint(activeBoxCollider.center);

                // 이전 Collider의 위치에서 현재 Collider 중심점을 향하는 방향 벡터를 계산
                var direction = (endPoint - prevColliderPos).normalized;

                // 이전 Collider 위치와 현재 Collider 중심 간의 거리 계산
                float distance = Vector3.Distance(prevColliderPos, endPoint);

                // BoxCollider의 절반 크기를 Local Scale에 맞게 변환하고 스케일 적용
                var halfExtents = Vector3.Scale(activeBoxCollider.size, activeBoxCollider.transform.localScale) * 0.5f;

                // Collider의 회전 정보를 저장 (월드 공간에서의 회전)
                var orientation = activeBoxCollider.transform.rotation;

                // 현재 Collider 중심에서 Box 크기만큼의 Overlap 체크를 수행 (충돌 감지)
                var checkCollision = Physics.OverlapBox(prevColliderPos, halfExtents, orientation, hitboxLayer);
                BoxCastDebug.DrawBoxCastBox(prevColliderPos, halfExtents, orientation, direction, distance, Color.red);
                // 충돌이 발생했으나 이전에 처리한 게임 오브젝트가 아닌 경우만 처리
                if (checkCollision.Length > 0 && prevGameObj != checkCollision[0].gameObject)
                {
                    // todo 충돌 대상의 부모 오브젝트에서 `IDamageable` 컴포넌트를 찾아 처리

                    if (checkCollision[0].TryGetComponent(out IDamageable damageable))
                        damageable.TakeDamage(playerInfo.totalAttack);

                    // 현재 충돌한 오브젝트를 기록하여 중복 처리 방지
                    prevGameObj = checkCollision[0].gameObject;
                }
                else
                {
                    // BoxCast를 사용하여 이전 위치에서 현재 위치로 이동한 동안 충돌이 있었는지 확인
                    // 충돌 감지는 특정 Layer와 트리거 상태를 포함하여 필터링
                    bool isHit = Physics.BoxCast(
                        prevColliderPos, // 캐스트 시작 위치
                        halfExtents, // Box 크기 (절반)
                        direction, // 캐스트 방향
                        out var hit, // 충돌 정보 저장
                        orientation, // 박스의 회전값
                        distance, // 캐스트 거리
                        hitboxLayer, // 충돌 레이어 필터
                        QueryTriggerInteraction.Collide // 트리거에 대한 충돌 감지 허용 여부
                    );

                    BoxCastDebug.DrawBoxCastBox(prevColliderPos, halfExtents, orientation, direction, distance, Color.red);


                    // 충돌이 발생하고 이전에 처리된 대상이 아닌 경우
                    if (isHit && prevGameObj != hit.transform.gameObject)
                    {
                        // todo 충돌 대상의 부모 오브젝트에서 `IDamageable` 컴포넌트를 찾아 처리
                        if (hit.transform.gameObject.TryGetComponent(out IDamageable damageable))
                            damageable.TakeDamage(playerInfo.totalAttack);

                        // 충돌한 게임 오브젝트를 기록하여 중복 처리 방지
                        prevGameObj = hit.transform.gameObject;
                    }
                }
            }

            // Collider가 활성화된 경우, 이전 Collider 위치를 현재 Collider 위치로 업데이트
            if (activeBoxCollider)
                prevColliderPos = activeBoxCollider.transform.TransformPoint(activeBoxCollider.center);
        }

        void EnableActiveCollider()
        {
            activeCollider = weaponCollider;
            OnEnableHit?.Invoke(currentWeaponHandler);
        }

        void DisableActiveCollider()
        {
            activeCollider = null;
        }

        public void EquipWeapon(WeaponData data)
        {
            UnEquipWeapon();

            data.InIt();
            SetWeaponObject(data);

            if (currentWeaponObject == null) return;
            weaponCollider = currentWeaponObject.GetComponentInChildren<BoxCollider>();
            EnableAndDisableWeapon(true);
        }
        public void UnEquipWeapon()
        {
            EnableAndDisableWeapon(false);
            currentWeaponObject = null;
            currentWeaponData = null;
        }
        void SetWeaponObject(WeaponData data)
        {
            var holder = animator.GetBoneTransform(HumanBodyBones.RightHand);
            currentWeaponObject = Instantiate(data.WeaponModel, holder, true);
            currentWeaponObject.transform.localPosition = data.LocalPosition;
            currentWeaponObject.transform.localRotation = Quaternion.Euler(data.LocalRotation);
            currentWeaponObject.tag = "Hitbox";
            currentWeaponHandler = currentWeaponObject.GetComponent<AttachedWeapon>();
            currentWeaponData = data;
            // Todo 무기 스왑 시스템 추가 할시 => 리스트 관리

        }

        void EnableAndDisableWeapon(bool enableWeapon)
        {
            currentWeaponObject?.SetActive(enableWeapon);
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

            // var attack = weaponData.AttacksContainer.Attacks[comboCount].Attack;
            // GUILayout.Label($"Attack Length: {attack.Clip.length}", debugStyle);
            // GUILayout.Label($"Attack Progress: {attackTimer.Progress}", debugStyle);
            // GUILayout.Label($"ImpactStart: {1f-attack.ImpactStartTime}", debugStyle);
            // GUILayout.Label($"ImpactEnd: {1f-attack.ImpactEndTime}", debugStyle);
            // GUILayout.Label($"Combo Count: {comboCount}", debugStyle);
            GUILayout.EndArea();
        }
    }
}