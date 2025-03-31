using UnityEngine;

namespace Timelesss
{
    public class HitState : PlayerState
    {
        readonly IState returnState;
        readonly StateMachine stateMachine;

        public HitState(PlayerController player, Animator animator, StateMachine stateMachine, IState returnState)
            : base(player, animator)
        {
            this.stateMachine = stateMachine;
            this.returnState = returnState;
        }

        public override void OnEnter()
        {
            player.ResetVelocity();
            player.RotatePlayerToTargetDirection(false);

            // AnimationSystem에서 콜백으로 상태 전이
            player.AnimationSystem.PlayOneShot(player.HitClip, () => {
                if (stateMachine.CurrentState == this)
                    stateMachine.ChangeState(returnState);
            });
        }

        public override void FixedUpdate()
        {
            player.ApplyGravity();
        }
    }
}