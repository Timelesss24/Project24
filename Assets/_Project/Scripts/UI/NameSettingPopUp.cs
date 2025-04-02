using System.Collections;
using System.Collections.Generic;
using Managers;
using Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class NameSettingPopUp : UIPopup
    {
        [SerializeField] private Button startButton;

        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TextMeshProUGUI waringText;

        private const string PlayerNameKey = "PlayerName";
        
        private const float WarningTextDuration = 3f;

        private void Start()
        {
            startButton.onClick.AddListener(OnClickStartButton);

            //PlayerPrefs.DeleteKey(PlayerNameKey); // 테스트 용

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
                StartCoroutine(ViewWarningText("이름은 최소 3글자 이상이어야 합니다."));
                return;
            }

            SavePlayerName(playerName);
            ClosePopup();
            GameStateManager.Instance.SetGameState(GameStateManager.GameState.Village);
        }

        private void SavePlayerName(string playerName)
        {
            PlayerPrefs.SetString(PlayerNameKey, playerName);
            PlayerPrefs.Save();
            Debug.Log($"닉네임 '{playerName}'이(가) 저장되었습니다.");
        }

        private IEnumerator ViewWarningText(string message)
        {
            waringText.text = message;
            waringText.color = Color.red;
            waringText.gameObject.SetActive(true);
            yield return new WaitForSeconds(WarningTextDuration);
            waringText.gameObject.SetActive(false);
        }
    }
}
