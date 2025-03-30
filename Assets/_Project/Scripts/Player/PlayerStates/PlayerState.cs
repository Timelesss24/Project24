using UnityEngine;

namespace Timelesss
{
    public abstract class PlayerState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int InAirHash = Animator.StringToHash("InAir");
        
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