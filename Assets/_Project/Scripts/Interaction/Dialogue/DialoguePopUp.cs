using Managers;
using Scripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class DialoguePopUp : UIPopup
    {
        [SerializeField]
        Button nextButton; // 다음 버튼
        [SerializeField]
        Button closeButton; // 닫기 버튼

        [SerializeField]
        TextMeshProUGUI dialogueText; // 대화 텍스트
        [SerializeField]
        TextMeshProUGUI npcNameText; // NPC 이름 텍스트

        Coroutine typewriterCoroutine; // 타자 효과 코루틴
        const float TypeWriterSpeed = 0.05f; // 타자 효과 속도

        PlayerInfo playerInfo;

        int npcID; // NPC ID

        void Awake()
        {
            playerInfo = FindObjectOfType<PlayerInfo>(); 

            if (playerInfo == null)
            {
                Debug.Log("playerInfo를 찾을 수 없습니다.");
            }

            nextButton.onClick.AddListener(OnClickNextButton); 
            closeButton.onClick.AddListener(OnClickCloseButton);
        }

        public void SetNpcID(int id) => npcID = id; // NPC ID 설정

        public void ShowDialogue(string text, bool hasNextDialogue, bool hasQuest, bool isComplete = false)
        {
            nextButton.gameObject.SetActive(false); // 다음 버튼 비활성화
            closeButton.gameObject.SetActive(false); // 닫기 버튼 비활성화

            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine); // 기존 코루틴 중지

            typewriterCoroutine =
                StartCoroutine(ChangeDialogueTextCoroutine(text, hasNextDialogue, hasQuest, isComplete)); // 새로운 코루틴 시작
        }

        IEnumerator ChangeDialogueTextCoroutine(string text, bool hasNextDialogue, bool hasQuest,
                                                bool isComplete = false)
        {
            dialogueText.text = ""; // 대화 텍스트 초기화

            text = text.Replace("모험가", $"{playerInfo.GetName()}"); // "모험가"를 플레이어 이름으로 대체

            foreach (char letter in text)
            {
                dialogueText.text += letter; // 한 글자씩 추가
                yield return new WaitForSeconds(TypeWriterSpeed);
            }

            if (hasNextDialogue) // 다음 대화가 있다면
            {
                nextButton.gameObject.SetActive(true); // 다음 버튼 활성화
            }
            else
            {
                HandleEndOfDialogue(hasQuest, isComplete); // 대화 종료 처리
            }
        }

        void HandleEndOfDialogue(bool hasQuest, bool isComplete)
        {
            if (isComplete)
            {
                ShowQuestPopup(isComplete: true); // 완료된 퀘스트 팝업 표시
            }
            else if (hasQuest)
            {
                ShowQuestPopup(isComplete: false); // 퀘스트 팝업 표시
            }
            else
            {
                closeButton.gameObject.SetActive(true); // 닫기 버튼 활성화
            }
        }

        void ShowQuestPopup(bool isComplete)
        {
            int questID = isComplete
                ? QuestManager.Instance.FindCompletedQuestID(npcID) // 완료된 퀘스트 ID 찾기
                : QuestManager.Instance.GetQuestID(npcID); // 퀘스트 ID 찾기

            if (questID == 0) return;

            QuestData questData = QuestManager.Instance.GetQuestData(questID); // 퀘스트 데이터 가져오기

            if (questData == null) return;

            AcceptQuestPopUp popUp = UIManager.Instance.ShowPopup<AcceptQuestPopUp>(); // 퀘스트 팝업 표시
            popUp.SetQuestInfo(questData, isComplete); // 퀘스트 정보 설정
        }

        void OnClickNextButton()
        {
            DialogueManager.Instance.ShowCurrentDialogue(); // 현재 대화 표시
        }

        void OnClickCloseButton()
        {
            ClosePopup(); // 팝업 닫기
        }

        public void SetNpcNameText(string name) => npcNameText.text = name; // NPC 이름 텍스트 설정
    }
}