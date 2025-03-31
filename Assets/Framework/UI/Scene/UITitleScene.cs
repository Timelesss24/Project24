using System;
using Managers;
using Scripts.UI;
using Timelesss;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.UI.Scene
{
    public class UITitleScene : UIScene
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button exitBtn;

        private void Awake()
        {
            if(startBtn)
                startBtn.onClick.AddListener(OnClickedStartBtn);
            if(settingBtn)
                settingBtn.onClick.AddListener(OnClickedSettingBtn);
            if(exitBtn)
                exitBtn.onClick.AddListener(OnClickedExitBtn);
        }

        private void OnClickedSettingBtn()
        {
            throw new NotImplementedException();
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
