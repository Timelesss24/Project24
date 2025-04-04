﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Timelesss
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        protected const string PlayerTag = "Player";

        public abstract string InteractionName { get; }

        [SerializeField]
        AnimationClip clip;
        public AnimationClip Clip => clip;
        

        [SerializeField] protected TextMeshPro interactionText;
        protected Coroutine textRotateCoroutine;


        public virtual void Interact()
        {
            InteractionManager.Instance.StartInteraction(this);
        }

        void Awake()
        {
            if (interactionText != null)
            {
                interactionText.text = InteractionName;
                interactionText.enabled = false;
            }
            else
            {
                Debug.LogWarning("TextMeshPro를 찾을 수 없습니다.");
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                other.GetComponent<PlayerInteractor>().AddInteractable(this);

                interactionText.enabled = true;

                if (textRotateCoroutine != null)
                    StopCoroutine(textRotateCoroutine);

                textRotateCoroutine = StartCoroutine(RotateInteractionText());
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                other.GetComponent<PlayerInteractor>().RemoveInteractable(this);

                interactionText.enabled = false;

                if (textRotateCoroutine != null)
                    StopCoroutine(textRotateCoroutine);
            }
        }

        IEnumerator RotateInteractionText()
        {
            while (interactionText.IsActive())
            {
                Vector3 directionToCamera = (Camera.main.transform.position - interactionText.transform.position).normalized;
                interactionText.transform.rotation = Quaternion.LookRotation(directionToCamera);
                interactionText.transform.eulerAngles += new Vector3(0, 180, 0);

                yield return null;
            }
        }
    }
}

