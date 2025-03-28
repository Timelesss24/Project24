using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "EnemyOS", menuName = "Enemy/EnemyOS")]
    public class EnemyOS : ScriptableObject
    {
        public float wanderRadius;
        public float timeBetweenAttacks;
        public float detectionAngle;
        public float detectionRadius;
        public float innerDetectionRadius;
        public float detectionCooldown;
        public float attackRange;
        public float attackDamage;
    }
}

