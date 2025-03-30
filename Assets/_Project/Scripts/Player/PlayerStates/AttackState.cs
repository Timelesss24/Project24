using UnityEngine;

namespace Timelesss
{
    public class AttackState : PlayerState
    {
        readonly CombatController combatController;
        readonly StateMachine stateMachine;
        readonly IState returnState;

        public AttackState(PlayerController player, Animator animator, CombatController combatController, StateMachine stateMachine, IState returnState)
            : base(player, animator)
        {
            this.combatController = combatController;
            this.stateMachine = stateMachine;
            this.returnState = returnState;
        }

        public override void OnEnter()
        {
            Debug.Log("AttackEnter");

            player.ResetVelocity();

            // 공격 종료 시 상태 복귀
            combatController.OnEndAttack += OnAttackEnd;
        }

        public override void OnExit()
        {
            // 리스너 해제
            combatController.OnEndAttack -= OnAttackEnd;
        }

        public override void FixedUpdate()
        {
            player.ApplyGravity();
        }

        void OnAttackEnd()
        {
            stateMachine.ChangeState(returnState);
        }
    }
}