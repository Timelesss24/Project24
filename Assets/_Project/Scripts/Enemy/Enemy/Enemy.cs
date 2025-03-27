using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timeless
{
    public class Enemy : MonoBehaviour
    {
        public EnemyStateMachine stateMachine;
        private void Awake()
        {
            stateMachine = new EnemyStateMachine(this);
        }
        void Start()
        {

        }
        void Update()
        {

        }
    }
}