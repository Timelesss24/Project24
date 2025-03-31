using UnityEngine;

namespace Timelesss
{
    public class DeathState : PlayerState
    {
        public DeathState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(DeathHash, crossFadeDuration);
            player.ResetVelocity();
        }

        public override void FixedUpdate()
        {
            player.ApplyGravity();
        }
    }
}