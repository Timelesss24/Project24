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
                StartCoroutine(RotateDoubleDoor());
            }
        }

        private IEnumerator RotateSingleDoor()
        {
            if (isOpen) yield break;

            isOpen = true;
            interactionText.text = string.Empty;

            float rotationTime = 0.5f;
            float time = 0f;

            float initialRotationY = transform.rotation.eulerAngles.y;
            float targetRotationY = transform.rotation.eulerAngles.y - 90f;

            while (time < rotationTime)
            {
                float newRotationY = Mathf.Lerp(initialRotationY, targetRotationY, time / rotationTime);

                transform.rotation = Quaternion.Euler(0, newRotationY, 0);

                time += Time.deltaTime;
                yield return null;
            }

            InteractionManager.Instance.EndInteraction();

            transform.rotation = Quaternion.Euler(0, targetRotationY, 0);
        }

        private IEnumerator RotateDoubleDoor()
        {
            if (isOpen) yield break;

            isOpen = true;
            interactionText.text = string.Empty;

            float rotationTime = 0.5f; 
            float time = 0f;

            List<Quaternion> initialRotations = new List<Quaternion>();
            List<Quaternion> targetRotations = new List<Quaternion>();

            for (int i = 0; i < doorTransforms.Length; i++)
            {
                initialRotations.Add(doorTransforms[i].localRotation);
                targetRotations.Add(Quaternion.Euler(0, i == 0 ? -90f : 90f, 0));
            }

            while (time < rotationTime)
            {
                for (int i = 0; i < doorTransforms.Length; i++)
                {
                    doorTransforms[i].localRotation = Quaternion.Lerp(
                        initialRotations[i],
                        targetRotations[i],
                        time / rotationTime
                    );
                }

                time += Time.deltaTime;
                yield return null;
            }

            for (int i = 0; i < doorTransforms.Length; i++)
            {
                doorTransforms[i].localRotation = targetRotations[i];
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
