using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class QuestPopUp : UIPopup
    {
        [SerializeField] Button closeButton;

        private QuestManager questManager;

        private void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseButton);

            questManager = QuestManager.Instance;

            foreach (int questID in questManager.ActiveQuestList)
            {
                QuestData data = questManager.GetQuestData(questID);

                Debug.Log($"현재 수락한 퀘스트 : {data.questName}");
            }
        }

        void OnClickCloseButton()
        {
            ClosePopup();
        }
    }
}
