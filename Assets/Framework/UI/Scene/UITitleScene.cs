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
        [SerializeField]
        Button startBtn;
        [SerializeField]
        Button settingBtn;
        [SerializeField]
        Button exitBtn;

        [SerializeField]
        TextMeshProUGUI titleText;

        [SerializeField]
        float wobbleAmount = 10f;
        [SerializeField]
        float wobbleSpeed = 0.6f;

        void Awake()
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

        void OnClickedSettingBtn()
        {
            UIManager.Instance.ShowPopup<SettingPopUp>();
        }

        void OnClickedStartBtn()
        {
            //GameStateManager.Instance.SetGameState(GameStateManager.GameState.Gameplay);\
            UIManager.Instance.ShowPopup<NameSettingPopUp>();
        }

        void OnClickedExitBtn()
        {
            Application.Quit();
        }
    }
}