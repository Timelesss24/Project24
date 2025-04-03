using Unity.VisualScripting;
using UnityEngine;

namespace Timelesss
{
    public class LocomotionState : PlayerState
    {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(LocomotionHash, crossFadeDuration);
        }

        public override void Update()
        {
            // call Player's move logic
            player.ApplyGravity();
            player.HandleMovement();
        }
        
    }
}