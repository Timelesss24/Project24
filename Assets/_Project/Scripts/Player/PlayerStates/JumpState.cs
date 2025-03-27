using UnityEngine;

namespace Timelesss
{
    public class JumpState : PlayerState
    {
        public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("Entering Jump State");
            animator.CrossFade(JumpHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // call Player's jump logic and move logic
            player.JumpAndGravity();
            player.HandleMovement();
        }
    }
}