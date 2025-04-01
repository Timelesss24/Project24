using Managers;
using Scripts.UI;
using Timelesss;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace _01_Scripts.UI.Scene
{
    public class UITitleScene : UIScene
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button exitBtn;

        [SerializeField] private TextMeshProUGUI titleText;

        [SerializeField] private float wobbleAmount = 10f;
        [SerializeField] private float wobbleSpeed = 0.6f;

        private void Awake()
        {
            if (startBtn)
                startBtn.onClick.AddListener(OnClickedStartBtn);
            if (settingBtn)
                settingBtn.onClick.AddListener(OnClickedSettingBtn);
            if (exitBtn)
                exitBtn.onClick.AddListener(OnClickedExitBtn);
            
            RectTransform rectTransform = titleText.GetComponent<RectTransform>();

            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + wobbleAmount, wobbleSpeed)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnClickedSettingBtn()
        {
            UIManager.Instance.ShowPopup<SettingPopUp>();
        }

        private void OnClickedStartBtn()
        {
            //GameStateManager.Instance.SetGameState(GameStateManager.GameState.Gameplay);\
            UIManager.Instance.ShowPopup<NameSettingPopUp>();
        }

        private void OnClickedExitBtn()
        {
            Application.Quit();
        }
    }
}