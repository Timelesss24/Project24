using Managers;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class SettingPopUp : UIPopup
    {
        [SerializeField] Button closeButton;

        void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);
        }

        void Start()
        {

        }

        void OnClickCloseButton()
        {
            ClosePopup();
        }
    }
}
