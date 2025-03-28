using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class PlayerInteractor : MonoBehaviour
    {
        private List<IInteractable> interactableList = new List<IInteractable>();

        [SerializeField] InputReader inputReader;

        private void OnEnable()
        {
            inputReader.InterAction += OnInteraction;
        }

        private void OnDisable()
        {
            inputReader.InterAction -= OnInteraction;
        }

        void OnInteraction()
        {
            if (interactableList.Count > 0)
            {
                IInteractable interactableObj = interactableList[interactableList.Count - 1];
                interactableObj.Interact();
                interactableList.Remove(interactableObj);
            }
        }

        public void AddInteractable(IInteractable interactable)
        {
            if (!interactableList.Contains(interactable))
            {
                interactableList.Add(interactable);
            }
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            if (interactableList.Contains(interactable))
            {
                interactableList.Remove(interactable);
                Debug.Log("remove");
            }
        }
    }
}
