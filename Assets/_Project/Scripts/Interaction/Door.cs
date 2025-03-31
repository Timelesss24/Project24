using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class Door : InteractableBase
    {
        enum State { Single, Double }


        public override string InteractionName => "¿­±â";

        [SerializeField] private State doorState;

        [SerializeField] private Transform[] doorTransforms;
        private Transform playerTransform;

        private bool isOpen;

        public override void Interact()
        {
            base.Interact();

            RotateDoor();
        }

        private void RotateDoor()
        {
            if (doorState == State.Single)
            {
                StartCoroutine(RotateSingleDoor());
            }
            else if (doorState == State.Double)
            {
                RotateDoubleDoor();
            }
        }

        private IEnumerator RotateSingleDoor()
        {
            if (isOpen) yield break;

            isOpen = true;
            interactionText.text = string.Empty;

            float rotationDuration = 0.5f;
            float elapsedTime = 0f;

            float initialRotationY = transform.rotation.eulerAngles.y;
            float targetRotationY = transform.rotation.eulerAngles.y - 90f;

            while (elapsedTime < rotationDuration)
            {
                float newRotationY = Mathf.Lerp(initialRotationY, targetRotationY, elapsedTime / rotationDuration);

                transform.rotation = Quaternion.Euler(0, newRotationY, 0);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            InteractionManager.Instance.EndInteraction();

            transform.rotation = Quaternion.Euler(0, targetRotationY, 0);
        }

        private void RotateDoubleDoor()
        {
            isOpen = true;

            foreach (var doorTransform in doorTransforms)
            {
                Vector3 directionToPlayer = playerTransform.position - doorTransform.position;

                float rotationY = directionToPlayer.x >= 0 ? -90f : 90f;

                doorTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
            }
            InteractionManager.Instance.EndInteraction();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (isOpen) return;

            if (other.CompareTag(PlayerTag))
                playerTransform = other.transform;

            base.OnTriggerEnter(other);
        }
    }
}
