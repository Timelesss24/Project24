using UnityEngine;

namespace Timelesss
{
    public class InteractState : PlayerState
    {
        PlayerInteractor playerInteractor;
        float rotationVelocity;

        public InteractState(PlayerController player, Animator animator, PlayerInteractor playerInteractor) : base(player, animator)
        {
            this.playerInteractor = playerInteractor;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            player.ResetVelocity();
        }
        
        public override void Update()
        {
            // 현재 상호작용 중인 물체의 위치
            var interactablePosition = playerInteractor.CurrentInteractable.transform.position;

            // 플레이어에서 상호작용 대상 방향 계산
            var targetDirection = (interactablePosition - player.transform.position).normalized;

            // 타겟 회전 각도 계산 (Y축으로만 회전)
            float targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

            // 부드러운 회전 각도 계산
            float smoothRotation = Mathf.SmoothDampAngle(
                player.transform.eulerAngles.y,        // 현재 Y축 회전값
                targetRotation,                        // 목표 회전값
                ref rotationVelocity,           // 참조할 회전 속도 값
                player.RotationSmoothTime              // 회전 딜레이 시간
            );

            // 회전 적용 (X, Z 고정, Y만 변경)
            player.transform.rotation = Quaternion.Euler(0f, smoothRotation, 0f);

            // 플레이어 중력 적용
            player.ApplyGravity();
        }
            
    }
}