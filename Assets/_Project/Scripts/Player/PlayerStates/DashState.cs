using UnityEngine;

namespace Timelesss
{
    public class DashState : PlayerState
    {
        public DashState(PlayerController player, Animator animator, GameObject prefab) : base(player, animator)
        {
            this.prefab = prefab;
        }

        GameObject prefab;

        public override void OnEnter()
        {
            animator.CrossFade(DashHash, crossFadeDuration);
            GameObject.Instantiate(prefab, player.transform.position, Quaternion.identity, player.transform);
        }

        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}