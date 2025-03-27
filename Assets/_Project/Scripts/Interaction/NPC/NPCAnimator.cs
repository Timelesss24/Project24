using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class NPCAnimator : MonoBehaviour
    {
        private Animator animator;

        private const string idleHashString = "IdleState";

        private int idleHash;

        private void Start()
        {
            animator = GetComponent<Animator>();

            idleHash = Animator.StringToHash(idleHashString);

            InvokeRepeating(nameof(SetRandomIdleAnimation), 0f, 5f);
        }

        private void SetRandomIdleAnimation()
        {
            int randomIdle = Random.Range(0, 4);
            animator.SetInteger(idleHash, randomIdle);
        }
    }
}
