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

        UIManager uIManager;

        private void Awake()
        {
            settingButton.onClick.AddListener(OnClickSettingButton);
            inventoryButton.onClick.AddListener(OnClickInventoryButton);
            questButton.onClick.AddListener(OnClickQuestButton);           
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
    }
}
