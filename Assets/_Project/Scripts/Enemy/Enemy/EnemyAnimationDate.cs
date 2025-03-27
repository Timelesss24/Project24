using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EnemyAnimationDate : MonoBehaviour
    {
        [SerializeField] private string moveParameterName = "Move";
        [SerializeField] private string speedParameterName = "Speed";
        [SerializeField] private string AttackParameterName = "Attack";
        [SerializeField] private string HitParameterName = "Hit";
        [SerializeField] private string DieParameterName = "Die";

        public int moveParameterHash { get; private set; }
        public int speedParameterHash { get; private set; }
        public int attackParameterHash { get; private set; }
        public int hitParameterHash { get; private set; }
        public int dieParameterHash { get; private set; }

        public void Initialize()
        {
            moveParameterHash = Animator.StringToHash(moveParameterName);
            speedParameterHash = Animator.StringToHash(moveParameterName);
            attackParameterHash = Animator.StringToHash(moveParameterName);
            hitParameterHash = Animator.StringToHash(moveParameterName);
            dieParameterHash = Animator.StringToHash(moveParameterName);
        }
    }
}
