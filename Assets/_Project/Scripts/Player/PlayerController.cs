using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using Platformer;
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
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;

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
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashDuration = 0.5f;
        [SerializeField] float dashCooldown = 2f;
        
        float rotationVelocity;
        float verticalVelocity;
        float currentSpeed;
        float dashVelocity = 1f;
        float animationBlend;
        
        bool isJumpPressed;
        bool isJumping;
        
        const float ZeroF = 0f;
        const float TerminalVelocity = 53.0f;

        
        Vector3 movement;
        Vector3 velocity;
        
        Transform mainCam;
        
        List<Timer> timers;
        CountdownTimer jumpCooldownTimer;
        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;

        StateMachine stateMachine;

        float Gravity=>-15f; // todo  캐릭터 공욜 인터페이스로 이동;        
        
        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            if (Camera.main != null) mainCam = Camera.main.transform;
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            // 타겟(플레이어)이 새로운 위치로 워프되었을 때, FreeLook 카메라의 위치를 업데이트함
            freeLookVCam.OnTargetObjectWarped(transform,
                transform.position - freeLookVCam.transform.position - Vector3.forward);

            SetupTimers();
            SetupStateMachine();

        }
        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            // Declare states
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            var dashState = new DashState(this, animator);

            // Define transitions
            At(locomotionState, jumpState, new FuncPredicate(() => isJumping));
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // Set initial state
            stateMachine.SetState(locomotionState);
        }
        bool ReturnToLocomotionState()
        {
            return groundChecker.IsGrounded
                   && !isJumping
                   && !dashTimer.IsRunning;
        }
        void SetupTimers()
        {
            // Setup timers
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);

            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);
            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () => {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };


            timers = new List<Timer> {jumpCooldownTimer, dashTimer, dashCooldownTimer };
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate conditions) => stateMachine.AddAnyTransition(to, conditions);

        void OnEnable()
        {
            input.Jump += OnJump;
            input.Dash += OnDash;
        }

        void OnDisable()
        {
            input.Jump -= OnJump;
            input.Dash -= OnDash;
        }

        void OnJump(bool performed)
        {
            if (!performed || isJumpPressed || jumpCooldownTimer.IsRunning || !groundChecker.IsGrounded) return;
            
            isJumpPressed = true;
            jumpCooldownTimer.Start();
        }

        void OnDash(bool performed)
        {
            if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
            {
                dashTimer.Start();
            }
        }

        void Start() => input.EnablePlayerActions();

        void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            stateMachine.Update();

            controller.Move(velocity.With(y: verticalVelocity) * Time.deltaTime);

            HandleTimers();
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void UpdateAnimator()
        {
             animator.SetFloat(Speed, animationBlend);
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        public void OnFootstep() { } // todo 애니메이션 이벤트 에러 방지용

        public void HandleJump()
        {
            if (!isJumpPressed) return; // 플레이어가 지면에 있는 경우
            
            // todo 낙하 타이머 초기화 
            
            // 점프 공식: H = 점프 높이, G = 중력 값
            // H * -2 * G 
            isJumpPressed = false;
            isJumping = true;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);
        }

        public void HandleMovement()
        {
            float targetSpeed = CalculateTargetSpeed();
            float currentHorizontalSpeed = CalculateCurrentHorizontalSpeed();
            UpdateSpeed(currentHorizontalSpeed, targetSpeed);
            SmoothAnimationBlend(targetSpeed);

            if (movement != Vector3.zero)
            {
                RotatePlayerToMovementDirection();
            }

            Vector3 targetDirection = CalculateTargetDirection();
            velocity = targetDirection * currentSpeed;
        }

        public void ApplyGravity()
        {
            if (groundChecker.IsGrounded)
            {
                if (verticalVelocity < 0.0f)
                {
                    isJumping = false;
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
        
        // ====== Helper Method =====
        float CalculateTargetSpeed()
        {
            return movement != Vector3.zero ? moveSpeed * dashVelocity : 0f;
        }

        float CalculateCurrentHorizontalSpeed()
        {
            // Compute magnitude for horizontal velocity only
            return new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        }

        void UpdateSpeed( float currentHorizontalSpeed, float targetSpeed)
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

        void RotatePlayerToMovementDirection()
        {
            float targetRotation = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, smoothRotation, 0.0f);
        }

        Vector3 CalculateTargetDirection()
        {
            float targetRotation = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
            return (Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward).normalized;
        }
        
        // 애니메이션 블렌딩 속도를 보간하여 부드럽게 변경

        void SmoothAnimationBlend(float targetSpeed)
        {
            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (animationBlend < 0.01f) animationBlend = 0f;
        }

    }

}