using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class PlayerInteractor : MonoBehaviour
    {
        private List<IInteractable> interactableList = new List<IInteractable>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (interactableList.Count > 0)
                {
                    IInteractable interactableObj = interactableList[interactableList.Count - 1];
                    interactableObj.Interact();
                    interactableList.Remove(interactableObj);
                }
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
            }
        }
    }
}
