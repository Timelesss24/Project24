using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Timelesss
{
    public class DialoguePopUp : UIPopup
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private Camera NPCCamera;

        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI npcNameText;


        private Coroutine typewriterCoroutine;
        private float typewriterSpeed = 0.05f;


        public void ShowDialogue(string text, bool hasNextDialogue)
        {
            nextButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);

            typewriterCoroutine = StartCoroutine(ChangeDialogueTextCoroutine(text, hasNextDialogue));
        }

        private IEnumerator ChangeDialogueTextCoroutine(string text, bool hasNextDialogue)
        {
            dialogueText.text = "";
            int totalCharacters = text.Length;

            foreach (char letter in text)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typewriterSpeed);
            }

            if (hasNextDialogue)
            {
                nextButton.gameObject.SetActive(true);
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(OnClickNextButton);
            }
            else
            {
                closeButton.gameObject.SetActive(true);
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnClickCloseButton);
            }
        }

        private void OnClickNextButton()
        {
            DialogueManager.Instance.ShowCurrentDialogue();
        }

        private void OnClickCloseButton()
        {
            ClosePopup();
        }

        public void SetNPCNameText(string name) => npcNameText.text = name;
    }
}
