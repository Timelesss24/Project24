using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Timelesss
{
    public class Slot : MonoBehaviour, IPointerDownHandler
    {
        [field: SerializeField] public Image Icon { get; private set; } // 슬롯에 표시될 아이템 이미지
        [field: SerializeField] public TextMeshProUGUI StackLabel { get; private set; } // 슬롯에 표시될 아이템 개수
        [field: SerializeField] public Image SlotFrame { get; private set; } // 슬롯의 프레임 이미지
        [field: SerializeField] public  Image DefaultIcon { get; private set; }
        
        public int Index { get; private set; } // 슬롯의 인덱스
        public SerializableGuid ItemId { get; private set; } = SerializableGuid.Empty;
        public Sprite BaseSprite { get; private set; } // 슬롯 기본 아이템 스프라이트
        public RectTransform RectTransform => GetComponent<RectTransform>();

        public event Action<Vector2, Slot> OnStartDrag = delegate { }; // 드래그 시작 이벤트

        //public Item item;
        
        /// <summary>
        /// 슬롯 초기화 메서드
        /// </summary>
        public void Initialize(int index)
        {
            Index = index; // 슬롯 인덱스 설정
            Remove(); // 초기 상태로 슬롯 리셋
        }

        /// <summary>
        /// 슬롯이 클릭되었을 때 처리
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || ItemId.Equals(SerializableGuid.Empty))
            {
                Debug.Log("Slot clicked, but no item in slot");
                return;
            }

            // 드래그 시작 이벤트 호출 (마우스 위치 및 현재 슬롯 전달)
            OnStartDrag?.Invoke(eventData.position, this);
        }
        

        /// <summary>
        /// 슬롯에 아이템 정보를 세팅
        /// </summary>
        public void Set(SerializableGuid id, Sprite iconSprite, int qty = 0)
        {
            ItemId = id; // 아이템 ID 설정
            BaseSprite = iconSprite; // 기본 스프라이트 설정

            // 아이콘 이미지 업데이트
            if (Icon)
            {
                Icon.sprite = BaseSprite;
                Icon.enabled = BaseSprite; // 아이콘이 없으면 비활성화
            }

            // 스택 라벨 업데이트
            if (StackLabel != null)
            {
                StackLabel.text = qty > 1 ? qty.ToString() : string.Empty;
                StackLabel.gameObject.SetActive(qty > 1);
            }
        }

        /// <summary>
        /// 슬롯에서 아이템 제거
        /// </summary>
        public virtual void Remove()
        {
            ItemId = SerializableGuid.Empty; // 슬롯 아이템 ID 초기화
            if (Icon != null)
            {
                Icon.sprite = null;
                Icon.enabled = false; // 아이콘 비활성화
            }

            if (StackLabel != null)
            {
                StackLabel.text = string.Empty;
                StackLabel.gameObject.SetActive(false); // 스택 라벨 숨김
            }
        }

        
        public void RestoreVisual()
        {
            if (BaseSprite != null)
            {
                Icon.sprite = BaseSprite;
                Icon.enabled = true;
            }

            if (StackLabel != null)
            {
                StackLabel.gameObject.SetActive(true);
            }
        }
    }
}