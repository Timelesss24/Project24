using UnityEngine;

namespace Timelesss
{
    public class DashState : PlayerState
    {
        public DashState(PlayerController player, Animator animator) : base(player, animator) { }
        
        public override void OnEnter()
        {
            animator.CrossFade(DashHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}