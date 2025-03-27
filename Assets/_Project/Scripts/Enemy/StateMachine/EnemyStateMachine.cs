using System.Collections;
using System.Collections.Generic;
using Platformer;
using UnityEngine;

namespace Timeless
{
    public class EnemyStateMachine : StateMachine
    {
        public Enemy Enemy { get; }

        public Vector2 Movement { get; set; }
        public float MovementSpeed { get; private set; }
        public float RotationDamping { get; private set; }
        public float MovemenSpeedModfier { get; set; }

        public GameObject Target { get; private set; }
        public EnemyAttackState AttackState { get; }
        public EnemyChasingState ChasingState { get; }
        public EnemyDieState DieStateState { get; }
        public EnemyIdleState IdleStateState { get; }

        public EnemyStateMachine(Enemy enemy)
        {
            this.Enemy = enemy;
            Target = GameObject.FindGameObjectWithTag("Player");

            AttackState = new EnemyAttackState(this);
            ChasingState = new EnemyChasingState(this);
            DieStateState = new EnemyDieState(this);
            IdleStateState = new EnemyIdleState(this);
        }
    }
}