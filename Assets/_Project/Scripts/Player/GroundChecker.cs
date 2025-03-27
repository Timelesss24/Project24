using UnityEngine;

namespace Timelesss
{
    public class GroundChecker : MonoBehaviour
    {
        [Tooltip("지면 체크에 사용될 원의 반지름 (CharacterController의 반지름과 일치해야 함)")]
        [SerializeField] float groundRadius = 0.28f;
        [Tooltip("지면 체크 시 오프셋 (불규칙한 지형에서 유용)")]
        [SerializeField] float groundOffset = -0.14f;
        [Tooltip("지면 체크에 사용될 원의 반지름 (CharacterController의 반지름과 일치해야 함)")]
        [SerializeField] LayerMask groundLayers;

        public bool IsGrounded { get; private set; }
        
        void Update()
        {
            // 지면 체크를 위한 구체(Sphere) 위치 설정
            var spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset,
                transform.position.z);
            IsGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        void OnDrawGizmosSelected()
        {
            var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = IsGrounded ? transparentGreen : transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z),
                groundRadius);
        }
    }
}