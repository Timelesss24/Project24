using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class ErathShake : MonoBehaviour
    {
        BossEnemy enemy;
        bool engage = false;

        void Start()
        {
            enemy = GetComponentInParent<BossEnemy>();
            InvokeRepeating("SkillDamage", 0, 1);
        }
        void OnTriggerEnter(Collider other)
        {
            engage = true;
        }
        void OnTriggerExit(Collider other)
        {
            engage = false;
        }
        void SkillDamage()
        {
            if (engage == false) return;
            if (enemy == null) return;

            enemy.Attack();
        }
    }
}
