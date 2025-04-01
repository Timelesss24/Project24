using UnityEngine;
using UnityEngine.EventSystems;

namespace Systems.Inventory
{
    public class PanelDragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        RectTransform rectTransform;
        Canvas canvas;
        Vector2 offset;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>(); // 현재 UI 요소의 RectTransform 가져오기
            canvas = GetComponentInParent<Canvas>(); // 상위 캔버스 가져오기
        }

        /// <summary>
        /// 드래그 시작 시 호출
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            // 드래그 시작 지점과 현재 UI 위치의 차이를 저장
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out offset
            );
        }

        /// <summary>
        /// 드래그 시작 시 호출(필요하지 않을 경우 비워놓을 수 있음)
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 드래그 시작 시 필요한 추가 로직 (선택적)
            Debug.Log("Begin Drag");
        }

        /// <summary>
        /// 드래그하는 동안 호출
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (rectTransform == null || canvas == null)
                return;

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPoint))
            {
                // 현재 마우스 위치 계산 및 UI 이동
                rectTransform.localPosition = localPoint - offset;
            }
        }

        /// <summary>
        /// 드래그 종료 시 호출(필요하지 않을 경우 비워놓을 수 있음)
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            // 드래그 종료 시 필요한 추가 로직 (선택적)
            Debug.Log("End Drag");
        }
    }
}