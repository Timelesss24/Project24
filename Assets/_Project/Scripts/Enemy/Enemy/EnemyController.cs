using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EnemyController : MonoBehaviour
    {
        [field: Header("Reference")]
        [field: SerializeField] public EnemyOS Date { get; private set; }

        [field:Header("Animations")]
        [field: SerializeField] public EnemyAnimationDate AnimationDate { get; private set; }

        public Animator Animator { get; private set; }
        
        private EnemyStateMachine stateMachine;

        private void Awake()
        {
            AnimationDate.Initialize();

            Animator = GetComponent<Animator>();

            stateMachine = new EnemyStateMachine(this);
        }
        void Start()
        {

        }
        void Update()
        {
            stateMachine.Update();
        }
        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
    }
}