using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class PlayerInteractor : MonoBehaviour
    {
        private List<IInteractable> interactableList = new List<IInteractable>();

        AnimationSystem animationSystem;
        [SerializeField] InputReader inputReader;

        private void Start()
        {
            animationSystem = GetComponent<PlayerController>().AnimationSystem;
        }

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
                animationSystem.PlayOneShot(interactableObj.Clip);
                interactableObj.Interact();

                if (interactableObj is DropItem)
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
            }
        }
    }
}
