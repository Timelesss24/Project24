using Unity.VisualScripting;
using UnityEngine;

namespace Timelesss
{
    public class InteractionTestPlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private CharacterController characterController;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = transform.AddComponent<CharacterController>();
            }
        }

        void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

            if (characterController != null)
            {
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
        }
    }
}

