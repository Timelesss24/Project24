using System;
using Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{

    public class ConfirmPopup : UIPopup
    {
        [Header("UI Elements")]
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] Button confirmButton;
        [SerializeField] Button cancelButton;

        public void InitailizePoup(string message, Action onConfirm, Action onCancel = null)
        {
            messageText.text = message;

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(() => {
                onConfirm?.Invoke();
                ClosePopup();
            });

            
            
            cancelButton.onClick.AddListener(() => {
                onCancel?.Invoke();
                ClosePopup();
            });
        }
    }
}