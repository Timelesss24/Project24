using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class NPCAnimator : MonoBehaviour
    {
        Animator animator;

        const string idleHashString = "IdleState";
        const string talkHashString = "IsTalk";

        [HideInInspector] public int idleHash;
        [HideInInspector] public int talkHash;

        void Start()
        {
            animator = GetComponent<Animator>();

            idleHash = Animator.StringToHash(idleHashString);
            talkHash = Animator.StringToHash(talkHashString);

            InvokeRepeating(nameof(SetRandomIdleAnimation), 0f, 5f);
        }

        void SetRandomIdleAnimation()
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
