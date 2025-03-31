using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Timelesss
{
    public class QuestSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI questNameText;
        [SerializeField] private Image completeCheckIcon;

        private QuestPopUp questPopup;
        private QuestData questData;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (questData != null)
                questPopup.SetQuestInfo(questData);
        }

        public void SetSlot(QuestData questData, QuestPopUp popUp)
        {
            this.questData = questData;
            questPopup = popUp;
            questNameText.text = this.questData.questName;

            completeCheckIcon.enabled = QuestManager.Instance.GetIsComplete(questData.key);
        }
    }
}
