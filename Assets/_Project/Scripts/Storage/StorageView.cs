using System.Collections;
using Scripts.UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Timelesss
{
    public interface IItemContainer
    {
        bool HandleDrop(Slot fromSlot, Slot toSlot, Item item);
    }
    
    public static class DragState
    {
        public static Slot OriginSlot { get; private set; }
        public static IItemContainer OriginContainer { get; private set; }
        public static Item DraggingItem { get; private set; }

        public static bool IsDragging => DraggingItem != null;

        public static void Begin(Slot slot, IItemContainer container, Item item)
        {
            OriginSlot = slot;
            OriginContainer = container;
            DraggingItem = item;
        }

        public static void Clear()
        {
            OriginSlot = null;
            OriginContainer = null;
            DraggingItem = null;
        }
    }
    
    public abstract class StorageView : UIPopup
    {
        public Slot[] Slots;
        [SerializeField] protected GameObject ghostIconObject;
       // [SerializeField] GameObject ghostIconObject;
        RectTransform ghostIcon;
        // public event Action<Slot, Slot> OnDrop;
        

        protected virtual void Start()
        {
            // GameObject의 RectTransform 가져오기
            if (ghostIconObject)
                ghostIcon = ghostIconObject.GetComponent<RectTransform>();
            
            
            ghostIcon.gameObject.SetActive(false); // 기본적으로 비활성화
            foreach (var slot in Slots)
            {
                slot.OnStartDrag += OnPointerDown;
            }
        }

        public abstract void InitializeView(int capacity = 0);
        
        // void OnPointerDown(Vector2 position, Slot slot)
        // {
        //     Debug.Log("OnPointerDown");
        //     isDragging = true;
        //     originalSlot = slot;
        //     StartCoroutine(FollowMouse());
        //     // Ghost 아이콘 초기화
        //     SetupGhostIcon(slot);
        // }

        protected abstract Item GetItemFromSlot(Slot slot);
        void OnPointerDown(Vector2 position, Slot slot)
        {
            Debug.Log("OnPointerDown");

            // 슬롯 인덱스를 이용해 아이템 조회
            Item item = GetItemFromSlot(slot);
            if (item == null || item.Id == SerializableGuid.Empty)
            {
                Debug.Log("드래그 시작 불가: 아이템 없음");
                return;
            }

            DragState.Begin(slot, this as IItemContainer, item);
            StartCoroutine(FollowMouse());

            SetupGhostIcon(slot);
        }
        
        // void Update()
        // {
        //     if (!isDragging) return;
        //     if (!Input.GetMouseButtonUp(0)) return;
        //     
        //     var closestSlot = Slots
        //         .Where(slot => RectTransformUtility.RectangleContainsScreenPoint(slot.RectTransform, Input.mousePosition))
        //         .OrderBy(slot => Vector2.Distance(slot.RectTransform.position, Input.mousePosition))
        //         .FirstOrDefault();
        //     if (closestSlot != null)
        //     {
        //         OnDrop?.Invoke(originalSlot, closestSlot);
        //     }
        //     else
        //     {
        //         // 원래 슬롯 아이콘 복구
        //         originalSlot.Icon.sprite = originalSlot.BaseSprite;
        //         originalSlot.Icon.enabled = true;
        //         originalSlot.StackLabel.gameObject.SetActive(true);
        //     }
        //     isDragging = false;
        //     originalSlot = null;
        //     ghostIcon.gameObject.SetActive(false); // Ghost 아이콘 숨김
        // }
        //
        //
        // IEnumerator FollowMouse()
        // {
        //     while (isDragging)
        //     {
        //         Vector2 mousePosition = Input.mousePosition;
        //  
        //         // Screen 좌표계를 UI 형식으로 변환
        //         RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //             ghostIcon.parent as RectTransform, 
        //             mousePosition,
        //             null, 
        //             out var uiPosition
        //         );
        //         // Ghost 아이콘 위치 지정
        //         SetGhostIconPosition(uiPosition);
        //         yield return null;
        //     }
        // }
        IEnumerator FollowMouse()
        {
            while (DragState.IsDragging)
            {
                Vector2 mousePosition = Input.mousePosition;
                RectTransform parent = ghostIcon.parent as RectTransform;

                if (parent == null)
                {
                    yield break;
                }

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parent,
                    mousePosition,
                    null, // 필요시 카메라 설정
                    out var uiPosition
                );

                SetGhostIconPosition(uiPosition);
                yield return null;
            }
        }
        // void SetupGhostIcon(Slot slot)
        // {
        //     ghostIcon.gameObject.SetActive(true);
        //     // Ghost 아이콘의 비주얼 설정
        //     var ghostImage = ghostIcon.GetComponent<Image>();
        //     ghostImage.sprite = slot.BaseSprite;
        //     originalSlot.Icon.sprite = null; // 드래그 시작 시 아이템 아이콘 숨기기
        //     originalSlot.Icon.enabled = false;
        //     originalSlot.StackLabel.gameObject.SetActive(false);
        // }

        void SetupGhostIcon(Slot slot)
        {
            ghostIcon.gameObject.SetActive(true);

            var ghostImage = ghostIcon.GetComponent<Image>();
            ghostImage.sprite = slot.BaseSprite;

            DragState.OriginSlot.Icon.sprite = null;
            DragState.OriginSlot.Icon.enabled = false;
            DragState.OriginSlot.StackLabel.gameObject.SetActive(false);
        }
        
        void SetGhostIconPosition(Vector2 position)
        {
            // RectTransform을 통해 위치를 설정
            ghostIcon.anchoredPosition = position;//- new Vector2(ghostIcon.sizeDelta.x / 2, ghostIcon.sizeDelta.y / 2);
        }
        
        public void HideGhostIcon()
        {
            if (ghostIcon != null)
                ghostIcon.gameObject.SetActive(false);
        }

    }
}
