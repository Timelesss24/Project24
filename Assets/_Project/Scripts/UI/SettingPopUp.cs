using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class SettingPopUp : UIPopup
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);
            exitButton.onClick.AddListener(OnClickExitButton);
        }

        private void OnClickCloseButton()
        {
            ClosePopup();
        }

        private void OnClickExitButton()
        {
            GameStateManager.Instance.QuitGame();
        }
    }
}