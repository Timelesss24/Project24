using UnityEngine;

namespace Timelesss
{
    public class InAir : PlayerState
    {
        public InAir(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(InAirHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // call Player's jump logic and move logic
            player.ApplyGravity();
        }
    }
}