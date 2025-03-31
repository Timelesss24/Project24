using System;
using System.Collections.Generic;
using Cinemachine;
using Core;
using KBCore.Refs;
using UnityEngine;
using UnityUtils;
using Utilities;
using Timer = Utilities.Timer;

namespace Timelesss
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] CharacterController controller;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField] Transform cameraLook;
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] CombatController combatController;
        [SerializeField, Self] PlayerInfo playerInfo;
        [SerializeField, Self] AnimationSystem animationSystem;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Self] PlayerInteractor playerInteractor;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 2f;
        [SerializeField] float rotationSpeed = 15f;
        [Tooltip("캐릭터가 이동 방향을 바라보는 속도")] [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("가속 및 감속 비율")]
        public float SpeedChangeRate = 10.0f;

        [Header("Jump Settings")]
        [SerializeField] float jumpHeight = 2;
        [SerializeField] float jumpCooldown;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] float fallTimeout = 0.15f;
        
        [Header("Dash Settings")]
        [SerializeField] float dashForce = 2f;
        [SerializeField] float dashStaminaCost = 10f;

        [Header("Roll Settings")]
        [SerializeField] float rollCooldown;
        [SerializeField] float rollStaminaCost = 20f;
        
        [Header("Exhausted Settings")]
        [SerializeField] float exhaustedDuration = 0.5f;

        float rotationVelocity;
        float verticalVelocity;
        float currentSpeed;
        float dashVelocity = 1f;
        float animationBlend;

        bool isSprint;
        bool isAttackPressed;
        bool isInteract;
        public bool IsRoll;
        public bool IsDie;

        public Vector3 Movement {get; private set;}
        Vector3 velocity;

        Transform mainCam;

        List<Timer> timers;
        CountdownTimer jumpCooldownTimer;
        CountdownTimer rollCooldownTimer;
        CountdownTimer exhaustedTimer;

        StateMachine stateMachine;

        Action onRoll;
        Action onInteract;
        Action onHit;

        public AnimationSystem AnimationSystem => animationSystem;

        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");
        const float ZeroF = 0f;
        const float TerminalVelocity = 53.0f;

        // todo Temp Code
        float Gravity => -15f;
        public AnimationClip RollClip;
        public AnimationClip HitClip;

        IState locomotionState;
        IState inAir;
        IState attackState;
        IState rollState;
        IState interactState;
        IState exhaustedState;
        IState hitState;
        IState deathState;

        IPredicate RollingPredicate => new FuncPredicate(() => IsRoll);
        IPredicate IsAttacking => new FuncPredicate(() => combatController.IsAttacking && stateMachine.CurrentState is not AttackState);

        bool IsBusy => stateMachine.CurrentState != locomotionState || animationSystem.IsSynced || IsDie; 
        public bool CanHit 
            => stateMachine.CurrentState != rollState 
               && combatController.AttackState != AttackStates.Impact 
               && stateMachine.CurrentState != hitState 
               && stateMachine.CurrentState != deathState; 

        void Awake()
        {
            if (Camera.main != null) mainCam = Camera.main.transform;
            freeLookVCam.Follow = cameraLook;
            freeLookVCam.LookAt = cameraLook;
            // 타겟(플레이어)이 새로운 위치로 워프되었을 때, FreeLook 카메라의 위치를 업데이트함
            freeLookVCam.OnTargetObjectWarped(cameraLook,
                cameraLook.position - freeLookVCam.transform.position - Vector3.forward);
            
            SetupTimers();
            SetupStateMachine();
        }
        
        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            // Declare states
             locomotionState = new LocomotionState(this, animator);
             inAir = new InAir(this, animator);
             attackState = new AttackState(this, animator, combatController, stateMachine, locomotionState);
             rollState = new RollState(this, animator,stateMachine, locomotionState);
             interactState = new InteractState(this, animator, playerInteractor);
             exhaustedState = new ExhaustedState(this, animator);
             hitState = new HitState(this, animator, stateMachine, locomotionState);
             deathState = new DeathState(this, animator);

            // Define transitions
            At(locomotionState,rollState, RollingPredicate);
            At(attackState,rollState, RollingPredicate);
            At(locomotionState, attackState, IsAttacking);
            At(attackState, locomotionState,new FuncPredicate( ()=>combatController.AttackState == AttackStates.Idle));
            At(locomotionState, interactState, new FuncPredicate(() => isInteract));
            At(interactState, locomotionState, new FuncPredicate(() => !isInteract));
            At(hitState,deathState, new FuncPredicate(() => IsDie));
            Any(exhaustedState, new FuncPredicate(() => exhaustedTimer.IsRunning));
            Any(hitState, new ActionPredicate(ref onHit));
            At(exhaustedState,locomotionState, new FuncPredicate(() => !exhaustedTimer.IsRunning));
            Any(inAir, new FuncPredicate(() => !groundChecker.IsGrounded));
            At(inAir, locomotionState, new FuncPredicate(() => groundChecker.IsGrounded));

            // Set initial state
            stateMachine.SetState(locomotionState);
        }
        bool ReturnToLocomotionState()
        {
            return groundChecker.IsGrounded
                   && !animationSystem.IsSynced;
        }
        void SetupTimers()
        {
            // Setup timers
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            rollCooldownTimer = new CountdownTimer(rollCooldown);
            exhaustedTimer = new CountdownTimer(exhaustedDuration);

            timers = new List<Timer> { jumpCooldownTimer, rollCooldownTimer, exhaustedTimer };
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate conditions) => stateMachine.AddAnyTransition(to, conditions);

        
        void Start()
        {
            input.EnablePlayerActions();

            InteractionManager.Instance.OnInteractionStart += interacterble => {
                if (interacterble.Clip) animationSystem.PlayOneShot(interacterble.Clip);
                isInteract = true;
            };
            InteractionManager.Instance.OnInteractionEnd += () => {isInteract = false;};
            playerInfo.ExhanstedAction += () => {
                exhaustedTimer.Start();
                isSprint = false;
            };

            playerInfo.OnDamageTaken += () => onHit.Invoke();
            playerInfo.DeathAction += () => {
                IsDie = true;
                input.DisablePlayerActions();
            };
        }
        
        void OnEnable()
        {
            input.Roll += OnRoll;
            input.Dash += OnDash;
            input.Attack += OnAttack;
            input.InterAction += OnInteraction;
        }

        void OnDisable()
        {
            input.Roll -= OnRoll;
            input.Dash -= OnDash;
            input.Attack -= OnAttack;
            input.InterAction -= OnInteraction;
        }
        void OnInteraction()
        {
            if (IsBusy) return;
            playerInteractor.TryInteraction();
        }

        void OnAttack()
        {
            if ((IsBusy && stateMachine.CurrentState != attackState) || Cursor.lockState != CursorLockMode.Locked) return;

            combatController?.TryAttack();
        }

        void OnRoll()
        {
            if (stateMachine.CurrentState == rollState || rollCooldownTimer.IsRunning ) return;
            if (!playerInfo.UseStamina(rollStaminaCost)) return;
            IsRoll = true;
            rollCooldownTimer.Start();
        }

        void OnDash(bool performed)
        {
            if (performed)
            {
                isSprint = true;
                dashVelocity = dashForce;
            }
            else
            {
                dashVelocity = 1f;
                isSprint = false;
            }
        }


        void Update()
        {
            Movement = new Vector3(input.Direction.x, ZeroF, input.Direction.y);
            stateMachine.Update();

            controller.Move(velocity.With(y: verticalVelocity) * Time.deltaTime);

            HandleTimers();
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void OnAnimatorMove()
        {
            HandleOnAnimationMove(animator);
        }

        void HandleOnAnimationMove(Animator animatorController)
        {
            if(stateMachine.CurrentState is  LocomotionState) return;
            if (animatorController.deltaPosition != Vector3.zero)
            {
                controller.Move(animatorController.deltaPosition);
            }
            transform.rotation *= animatorController.deltaRotation;
        }

        void UpdateAnimator()
        {
            animator.SetFloat(Speed, velocity.With(y:0f).magnitude / moveSpeed);
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        public void OnFootstep() { } // todo 애니메이션 이벤트 에러 방지용

        public void JumpPressed()
        {
            // todo 낙하 타이머 초기화 

            // 점프 공식: H = 점프 높이, G = 중력 값
            // H * -2 * G 
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);
        }

        public void HandleMovement()
        {
            float targetSpeed = CalculateTargetSpeed();
            float currentHorizontalSpeed = CalculateCurrentHorizontalSpeed();
            UpdateSpeed(currentHorizontalSpeed, targetSpeed);

            if (Movement != Vector3.zero)
            {
                RotatePlayerToTargetDirection();
            }

            Vector3 targetDirection = CalculateTargetDirection();
            velocity = targetDirection * currentSpeed;
        }

        public void ApplyGravity()
        {
            if (groundChecker.IsGrounded)
            {
                if (verticalVelocity < ZeroF)
                {
                    verticalVelocity = -2f; // 살짝 음수 값을 줘서 지면에 붙어 있게 함
                }
            }
            else
            {

                // todo 낙하 타이머 >> 착지 로직 
            }
            // 중력 적용 (터미널 속도까지 증가)
            if (verticalVelocity < TerminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        // ====== Helper Method ====

        float CalculateTargetSpeed()
        {
            if(Movement == Vector3.zero) return ZeroF;
            
           
                if (isSprint)
                {
                    playerInfo.UseStamina(dashStaminaCost * Time.deltaTime);
                    return moveSpeed * dashVelocity;
                }
                
            
            return moveSpeed;
        }

        float CalculateCurrentHorizontalSpeed()
        {
            // Compute magnitude for horizontal velocity only
            return new Vector3(controller.velocity.x, ZeroF, controller.velocity.z).magnitude;
        }

        void UpdateSpeed(float currentHorizontalSpeed, float targetSpeed)
        {
            const float speedOffset = 0.1f;

            if (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > speedOffset)
            {
                currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                currentSpeed = Mathf.Round(currentSpeed * 1000f) / 1000f; // Round to 3 decimal places
            }
            else
            {
                currentSpeed = targetSpeed;
            }

        }

        public void RotatePlayerToTargetDirection(bool isSmooth = true)
        {
            // 주어진 타겟 방향을 기준으로 회전 각도 계산
            float targetRotation = Mathf.Atan2(Movement.x, Movement.z) * Mathf.Rad2Deg
                                   + mainCam.transform.eulerAngles.y;

            float smoothRotation = targetRotation;

            // 부드러운 회전 여부 확인
            if (isSmooth)
            {
                smoothRotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetRotation,
                    ref rotationVelocity,
                    RotationSmoothTime
                );
            }

            // 최종적으로 회전 적용
            transform.rotation = Quaternion.Euler(ZeroF, smoothRotation, ZeroF);
        }

       public  Vector3 CalculateTargetDirection()
        {
            float targetRotation = Mathf.Atan2(Movement.x, Movement.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
            return (Quaternion.Euler(ZeroF, targetRotation, ZeroF) * Vector3.forward).normalized;
        }
       

        public void ResetVelocity()
        {
            velocity = Vector3.zero;
        }
    }

}