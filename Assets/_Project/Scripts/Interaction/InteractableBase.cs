using UnityEngine;

namespace Timelesss
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        protected const string PlayerTag = "Player";

        public abstract string InteractionName { get; }

        public abstract void Interact();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                other.GetComponent<PlayerInteractor>().AddInteractable(this);
                Debug.Log(InteractionName);

                // UI ������Ʈ ����ֱ� (InteractionName)
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                other.GetComponent<PlayerInteractor>().RemoveInteractable(this);
                Debug.Log("OnTriggerExit");

                // UI ������Ʈ �����ֱ�
            }
        }
    }
}

