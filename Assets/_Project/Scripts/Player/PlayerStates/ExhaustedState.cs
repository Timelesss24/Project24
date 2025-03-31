using UnityEngine;

namespace Timelesss
{
    public class ExhaustedState : PlayerState
    {
        public ExhaustedState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(ExhaustedHash, crossFadeDuration);
            player.ResetVelocity();
        }

        public override void FixedUpdate()
        {
            player.ApplyGravity();
        }
    }
}