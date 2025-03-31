using System;
using UnityEngine;
using UnityUtils;

namespace Timelesss
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        public event Action<InteractableBase> OnInteractionStart;
        public event Action OnInteractionEnd;

        public void StartInteraction(InteractableBase interactable)
        {
            OnInteractionStart?.Invoke(interactable);
        }

        public void EndInteraction()
        {
            OnInteractionEnd?.Invoke();
        }
    }
}