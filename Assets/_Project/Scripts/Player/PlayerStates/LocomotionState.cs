using UnityEngine;

namespace Timelesss
{
    public class LocomotionState : PlayerState
    {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("Entering Locomotion State");
           // animator.CrossFade(LocomotionHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // call Player's move logic
            player.HandleMovement();
        }
    }
}