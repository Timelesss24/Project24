using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class ErathShake : MonoBehaviour
    {
        BossEnemy enemy;
        bool engage = false;

        private void Start()
        {
            enemy = GetComponentInParent<BossEnemy>();
            InvokeRepeating("SkillDamage", 0, 1);
        }
        private void OnTriggerEnter(Collider other)
        {
            engage = true;
        }
        private void OnTriggerExit(Collider other)
        {
            engage = false;
        }
        void SkillDamage()
        {
            if (engage == false) return;

            enemy.Attack();
        }
    }
}
