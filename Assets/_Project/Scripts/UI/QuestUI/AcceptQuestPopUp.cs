using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class AcceptQuestPopUp : UIPopup
    {
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;

        [SerializeField] private TextMeshProUGUI questNameText;
        [SerializeField] private TextMeshProUGUI questDescriptionText;
        [SerializeField] private TextMeshProUGUI questTargetText;
        [SerializeField] private TextMeshProUGUI questRewardText;
    }
}
