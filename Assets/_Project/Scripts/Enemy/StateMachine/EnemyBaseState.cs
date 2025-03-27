using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EnemyBaseState : IState
    {
        protected EnemyStateMachine stateMachine;

        public EnemyBaseState(EnemyStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
        //protected void StartAnimation(int ani)
    }
}
