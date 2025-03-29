using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "EnemyOS", menuName = "Enemy/EnemyOS")]
    public class EnemyOS : ScriptableObject
    {
        public float wanderRadius = 10f;
        public float timeBetweenAttacks = 1f;
        public float detectionAngle = 60f;
        public float detectionRadius = 10f;
        public float innerDetectionRadius = 5f;
        public float detectionCooldown = 1f;
        public float attackRange = 2f;
        public float attackDamage = 10f;
    }
}

