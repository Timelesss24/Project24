using DG.Tweening;
using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class GameOverPopUp : UIPopup
    {
        [SerializeField] GameObject gameOverButton;
        [SerializeField] TextMeshProUGUI gameOverText;

        private void Awake()
        {
            gameOverButton.GetComponent<Button>().onClick.AddListener(OnClickGameOverButton);
        }

        private void Start()
        {
            StartCoroutine(WaitTime());
        }

        IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(3f);
            gameOverText.DOFade(1, 1);
            gameOverButton.transform.GetComponent<Image>().DOFade(1, 3);
        }

        void OnClickGameOverButton()
        {
            GameStateManager.Instance.SetGameState(GameStateManager.GameState.Village);
        }

    }
}
