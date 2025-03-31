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

        public override void Interact()
        {
            base.Interact();

            RotateDoor();

            InteractionManager.Instance.EndInteraction();
        }

        private void RotateDoor()
        {
            if (doorState == State.Single)
            {
                RotateSingleDoor();
            }
            else if (doorState == State.Double)
            {
                RotateDoubleDoor();
            }
        }

        private void RotateSingleDoor()
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;

            float rotationY = directionToPlayer.x >= 0 ? -180f : 180f;

            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }

        private void RotateDoubleDoor()
        {
            foreach (var doorTransform in doorTransforms)
            {
                Vector3 directionToPlayer = playerTransform.position - doorTransform.position;

                float rotationY = directionToPlayer.x >= 0 ? -180f : 180f;

                doorTransform.rotation = Quaternion.Euler(0, rotationY, 0);
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerTag))
                playerTransform = other.transform;

            base.OnTriggerEnter(other);
        }
    }
}
