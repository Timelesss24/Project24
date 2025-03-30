using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;

namespace Timelesss
{
    public class CombatEffect : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] CombatController combatController;

        void OnValidate()=> this.ValidateRefs();
        


        void Start()
        {
            combatController.OnEnableHit += (AttachedWeapon handler) => StartCoroutine(EnableTrail(handler));
        }
        IEnumerator EnableTrail(AttachedWeapon handler)
        {
            if (handler?.Trail == null) yield break;

            handler.Trail.gameObject.SetActive(true);
            var impactEndTime = combatController.CurrentAttack.ImpactEndTime * combatController.CurrentAttack.Clip.length;
            float timer = 0;
            while (timer < impactEndTime)
            {
                if (combatController.AttackState != AttackStates.Impact)
                    break;
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            handler.Trail.gameObject.SetActive(false);
        }


    }
}