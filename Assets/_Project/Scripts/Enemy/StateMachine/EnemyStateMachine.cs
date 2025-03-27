using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EnemyStateMachine : StateMachine
    {
        public EnemyController Enemy { get; }

        public Vector2 Movement { get; set; }
        public float MovementSpeed { get; private set; }
        public float RotationDamping { get; private set; }
        public float MovemenSpeedModfier { get; set; }

        public GameObject Target { get; private set; }
        public EnemyAttackState AttackState { get; }
        public EnemyChasingState ChasingState { get; }
        public EnemyDieState DieStateState { get; }
        public EnemyIdleState IdleStateState { get; }

        public EnemyStateMachine(EnemyController enemy)
        {
            this.Enemy = enemy;
            Target = GameObject.FindGameObjectWithTag("Player");

            AttackState = new EnemyAttackState(this);
            ChasingState = new EnemyChasingState(this);
            DieStateState = new EnemyDieState(this);
            IdleStateState = new EnemyIdleState(this);

            MovementSpeed = Enemy.Date.BaseSpeed;
            RotationDamping = Enemy.Date.BaseRotationDamping;
        }
    }
}