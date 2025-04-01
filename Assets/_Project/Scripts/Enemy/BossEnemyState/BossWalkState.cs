using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Timelesss
{
    public class BossWalkState : BossBaseState
    {
        readonly NavMeshAgent agent;
        readonly Transform target;
        
        public BossWalkState(BossEnemy enemy, Animator animator, NavMeshAgent agent, Transform target)
            : base(enemy, animator) 
        {
            this.agent = agent;
            this.target = target;
        }
        public override void OnEnter()
        {
            
            animator.CrossFade(WalkHash, crossFadeDuration);
        }
        public override void Update()
        {
            
            agent.SetDestination(target.position);
        }
        public override void OnExit()
        {
            agent.ResetPath();
        }
    }
}
