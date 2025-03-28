using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class NPCAnimator : MonoBehaviour
    {
        private Animator animator;

        private const string idleHashString = "IdleState";
        private const string talkHashString = "IsTalk";

        public int idleHash;
        public int talkHash;

        private void Start()
        {
            animator = GetComponent<Animator>();

            idleHash = Animator.StringToHash(idleHashString);
            talkHash = Animator.StringToHash(talkHashString);

            InvokeRepeating(nameof(SetRandomIdleAnimation), 0f, 5f);
        }

        private void SetRandomIdleAnimation()
        {
            int randomIdle = Random.Range(0, 4);
            animator.SetInteger(idleHash, randomIdle);
        }

        public void StartAnimation(int hash)
        {
            animator.SetBool(hash, true);
        }

        public void StopAnimation(int hash)
        {
            animator.SetBool(hash, false);
        }
    }
}
