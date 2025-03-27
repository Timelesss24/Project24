using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "EnemyOS", menuName = "Enemy/EnemyOS")]
    public class EnemyOS : ScriptableObject
    {
        [field: SerializeField] public float PlayerChasingRange { get; private set; }
        [field: SerializeField] public float AttackRange { get; private set; }
        [field: SerializeField] public float BaseSpeed { get; private set; }
        [field: SerializeField] public float BaseRotationDamping { get; private set; }
        [field: SerializeField][field: Range(0f, 3f)] public float ForceTransitionTime { get; private set; }
        [field: SerializeField][field: Range(-10f, 10f)] public float Force { get; private set; }
        [field: SerializeField] public int Damage;
        [field: SerializeField][field: Range(0f, 1f)] public float DealingStartTransitionTime { get; private set; }
        [field: SerializeField][field: Range(0f, 1f)] public float DealingEndTransitionTime { get; private set; }
    }
}

