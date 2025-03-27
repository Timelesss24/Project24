using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using Platformer;
using UnityEngine;
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
        [SerializeField, Child] Animator animator;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;

        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown;
        [SerializeField] float gravityMultiplier = 3f;

        [Header("Dash Settings")]
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashDuration = 1f;
        [SerializeField] float dashCooldown = 2f;


        //===================
        [Tooltip("캐릭터가 이동 방향을 바라보는 속도")] [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("가속 및 감속 비율")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("플레이어가 점프할 수 있는 최대 높이")] [Space(10)]
        public float JumpHeight = 1.2f;
        [Tooltip("캐릭터가 사용할 자체 중력 값 (엔진 기본값은 -9.81f)")]
        public float Gravity = -15.0f;
        [Tooltip("점프 후 다시 점프할 수 있기까지 필요한 시간 (0f으로 설정하면 즉시 점프 가능)")] [Space(10)]
        public float JumpTimeout = 0.50f;
        [Tooltip("캐릭터가 낙하 상태로 진입하기 전까지 걸리는 시간 (계단 내려갈 때 유용)")]
        public float FallTimeout = 0.15f;
        
        // 플레이어 이동 관련 변수
         float speed;
         //float targetRotation = 0.0f;
         float rotationVelocity;
         float verticalVelocity;

        const float ZeroF = 0f;
        readonly float terminalVelocity = 53.0f;
        
        Transform mainCam;

        float currentSpeed;
        float velocity;
        float jumpVelocity;
        float dashVelocity = 1f;

        Vector3 movement;

        List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;

        StateMachine stateMachine;

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
            At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // Set initial state
            stateMachine.SetState(locomotionState);
        }
        bool ReturnToLocomotionState()
        {
            return groundChecker.IsGrounded
                   && !jumpTimer.IsRunning
                   && !dashTimer.IsRunning;
        }
        void SetupTimers()
        {
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);
            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () => {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };


            timers = new List<Timer> { jumpTimer, jumpCooldownTimer, dashTimer, dashCooldownTimer };
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
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }

        void OnDash(bool performed)
        {
            if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
            {
                dashTimer.Start();
            }
            else if (!performed && dashTimer.IsRunning)
            {
                dashTimer.Stop();
            }
        }

        void Start() => input.EnablePlayerActions();

        void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            stateMachine.Update();

            HandleTimers();
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void UpdateAnimator()
        {
            // animator.SetFloat(Speed, currentSpeed);
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void JumpAndGravity()
        {
            // If not jumping and Grounded, keep jump velocity at 0
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = ZeroF;
                return;
            }

            if (!jumpTimer.IsRunning)
            {
                // Gravity takes over
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            // Apply the calculated velocity to the player
            //rb.velocity = new Vector3(rb.velocity.x, jumpVelocity * gravityMultiplier, rb.velocity.z);
        }

        public void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);
            }
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the Player
            var horizontalVelocity = adjustedDirection * (moveSpeed * dashVelocity * Time.fixedDeltaTime);
            controller.Move(horizontalVelocity);
        }

        void HandleRotation(Vector3 adjustedDirection)
        {
        // 기준 방향의 Y 축 회전값을 Quaternion에서 각도로 변환
        float targetRotation = Mathf.Atan2(adjustedDirection.x, adjustedDirection.z) * Mathf.Rad2Deg;

        // 현재 회전값과 목표 회전값 간의 변화값을 부드럽게 계산 (SmoothDamp 사용)
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

        // 계산된 회전값을 사용하여 오브젝트의 회전을 설정
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        }

        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), $"DashTimer: {dashTimer.Progress}");
        }
    }
}