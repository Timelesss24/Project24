using UnityEngine;

namespace Timelesss
{
    public class AttackState : PlayerState
    {
        CombatController combatController;
        public AttackState(PlayerController player, Animator animator, CombatController combatController) : base(player, animator)
        {
            this.combatController = combatController;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            player.ResetVelocity();
        }

        public override void Update()
        {
            combatController.HandleAttack();
        }
    }
}