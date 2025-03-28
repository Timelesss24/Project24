using DG.Tweening;
using KBCore.Refs;
using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class MainUI : UIScene
    {
        [SerializeField] Button settingButton;
        [SerializeField] Button inventoryButton;
        [SerializeField] Button questButton;

        [SerializeField] Button skillButton;
        [SerializeField] Image sbCoolTimeIndicator;
        [SerializeField] Button ultimateSkillButton;
        [SerializeField] Image usbCoolTimeIndicator;
        [SerializeField] Button useItemButton;
        [SerializeField] Image uibCoolTimeIndicator;

        UIManager uIManager;

        private void Awake()
        {
            settingButton.onClick.AddListener(OnClickSettingButton);
            inventoryButton.onClick.AddListener(OnClickInventoryButton);
            questButton.onClick.AddListener(OnClickQuestButton);
            skillButton.onClick.AddListener(OnClickSkillButton);
            ultimateSkillButton.onClick.AddListener(OnClickUltimateSkillButton);
            useItemButton.onClick.AddListener(OnClickUseItemButton);
        }

        private void Start()
        {
            uIManager = UIManager.Instance;
        }

        void OnClickSettingButton()
        {
            uIManager.ShowPopup<SettingPopUp>();
        }

        void OnClickInventoryButton()
        {
            uIManager.ShowPopup<InventoryPopUp>();
            //eventChannel.Invoke(10);
        }

        void OnClickQuestButton()
        {
            uIManager.ShowPopup<QuestPopUp>();
        }
        void OnClickSkillButton()
        {
            CoolTime(1f, sbCoolTimeIndicator);
        }
        void OnClickUltimateSkillButton()
        {
            CoolTime(3f, usbCoolTimeIndicator);
        }
        void OnClickUseItemButton()
        {
            CoolTime(5f, uibCoolTimeIndicator);
        }

        void CoolTime(float time, Image indicator)
        {
            indicator.fillAmount = 1f;
            indicator.DOFillAmount(0, time);
        }
    }
}
