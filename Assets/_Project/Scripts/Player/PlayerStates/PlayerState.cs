using Timelesss;
using UnityEngine;

namespace Timelesss
{
    public abstract class PlayerState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");
        protected static readonly int DashHash = Animator.StringToHash("Dash");
        protected static readonly int AttackHash = Animator.StringToHash("Attack");
        
        protected const float crossFadeDuration = 0.1f;

        protected PlayerState(PlayerController player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }
        
        public virtual void OnEnter()
        {
            Debug.Log("Entering state: " + this.GetType().Name);
        }
        public virtual void Update()
        {
            // noop
        }
        public virtual void FixedUpdate()
        {
            // noop
        }
        public virtual void OnExit()
        {
            Debug.Log("Exiting state: " + this.GetType().Name);
        }
    }

}