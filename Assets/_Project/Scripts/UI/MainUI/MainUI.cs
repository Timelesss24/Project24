using DG.Tweening;
using Managers;
using Scripts.UI;
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

        [SerializeField] GameObject bossUI;

        UIManager uIManager;

        private void Awake()
        {
            settingButton.onClick.AddListener(OnClickSettingButton);
            inventoryButton.onClick.AddListener(OnClickInventoryButton);
            questButton.onClick.AddListener(OnClickQuestButton);
            skillButton.onClick.AddListener(OnClickSkillButton);
            ultimateSkillButton.onClick.AddListener(OnClickUltimateSkillButton);
            useItemButton.onClick.AddListener(OnClickUseItemButton);
            DungeonManager.Instance.bossHpUIAction += SetBossUI;
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
            var view = uIManager.ShowPopup<InventoryView>();
            view.Bind(PlayerManager.Instance.Inventory.Controller);
            PlayerManager.Instance.Inventory.Controller.InitializeView(view);
            
            var eqView = uIManager.ShowPopup<EquipmentView>();
            eqView.Bind(PlayerManager.Instance.Equipment.Controller);
            PlayerManager.Instance.Equipment.Controller.InitializeView(eqView);
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

        public void SetBossUI()
        {
            bossUI.SetActive(true);
        }
    }
}
