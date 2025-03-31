using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class NameSettingPopUp : UIPopup
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private TMP_InputField nameInput;

        private const string PlayerNameKey = "PlayerName";

        private void Start()
        {
            startButton.onClick.AddListener(OnClickStartButton);
            closeButton.onClick.AddListener(OnClickCloseButton);

            if (PlayerPrefs.HasKey(PlayerNameKey))
            {
                ClosePopup();
                GameStateManager.Instance.SetGameState(GameStateManager.GameState.Village);
                return;
            }
        }

        private void OnClickStartButton()
        {
            string playerName = nameInput.text.Trim();

            if (string.IsNullOrEmpty(playerName) || playerName.Length < 3)
            {
                Debug.LogWarning("이름은 최소 3글자 이상이어야 합니다.");
                return;
            }

            SavePlayerName(playerName);
            ClosePopup();
            GameStateManager.Instance.SetGameState(GameStateManager.GameState.Village);
        }

        private void OnClickCloseButton()
        {
            ClosePopup();
        }

        private void SavePlayerName(string playerName)
        {
            PlayerPrefs.SetString(PlayerNameKey, playerName);
            PlayerPrefs.Save();
            Debug.Log($"닉네임 '{playerName}'이(가) 저장되었습니다.");
        }
    }
}
