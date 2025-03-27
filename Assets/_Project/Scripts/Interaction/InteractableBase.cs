using UnityEngine;

namespace Timelesss
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        protected const string PlayerTag = "Player";

        public abstract string InteractionName { get; }

        public abstract void Interact();

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                other.GetComponent<PlayerInteractor>().AddInteractable(this);
                Debug.Log(InteractionName);

                // UI 프롬프트 띄워주기 (InteractionName)
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                other.GetComponent<PlayerInteractor>().RemoveInteractable(this);
                Debug.Log("OnTriggerExit");

                // UI 프롬프트 지워주기
            }
        }
    }
}

