using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class InventoryPopUp : UIPopup
    {
        [SerializeField] Button closeButton;

        private void Start()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);
        }

        void OnClickCloseButton()
        {
            ClosePopup();
        }
    }
}
