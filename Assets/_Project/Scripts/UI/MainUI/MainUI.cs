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
        [SerializeField] Button ultimateSkillButton;
        [SerializeField] Button useItemButton;
        [SerializeField] EventChannel<float> eventChannel;
        

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
            eventChannel.Invoke(10);
        }
        void OnClickQuestButton()
        {
            uIManager.ShowPopup<QuestPopUp>();
        }
        void OnClickSkillButton() 
        { 

        }
        void OnClickUltimateSkillButton()
        {

        }
        void OnClickUseItemButton()
        {

        }
    }
}
