using System;
using Core;
using KBCore.Refs;
using UnityEngine;

namespace Timelesss
{
    public class CombatController : MonoBehaviour
    {
        [SerializeField] PlayerController playerController; 
        [SerializeField] WeaponData weapon;
        
        AnimationSystem animationSystem;

        int comboCount;

        void Start()
        {
            weapon.InIt();
            animationSystem = playerController.AnimationSystem;
        }

        public void HandleAttack()
        {
            var clip = weapon.AttacksContainer.Attacks[comboCount].Attack.Clip;

            animationSystem.PlayOneShot(clip);
        }
    }
}