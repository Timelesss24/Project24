using UnityEngine;

namespace Timelesss
{
    public class RollState : PlayerState
    {
        readonly IState returnState;
        readonly StateMachine stateMachine;

        public RollState(PlayerController player, Animator animator, StateMachine stateMachine, IState returnState)
            : base(player, animator)
        {
            this.stateMachine = stateMachine;
            this.returnState = returnState;
        }

        public override void OnEnter()
        {
            Debug.Log("RollEnter");

            player.ResetVelocity();
            player.RotatePlayerToMovementDirection(false);

            // AnimationSystem에서 콜백으로 상태 전이
            player.AnimationSystem.PlayOneShot(player.RollClip, () =>
            {
                stateMachine.ChangeState(returnState);
            });
        }

        public override void FixedUpdate()
        {
            player.ApplyGravity();
        }
    }
}