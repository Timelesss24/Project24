using UnityEngine;

namespace Timelesss
{
    public class AttackState : PlayerState
    {
        CombatController combatController;
        public AttackState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            base.OnEnter();
            player.ResetVelocity();
        }
    }
}