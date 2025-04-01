using System.Linq;
using UnityEngine;

namespace Timelesss
{
    public class UIDragHandler : MonoBehaviour
    {
        void Update()
        {
            if (!DragState.IsDragging) return;

            if (Input.GetMouseButtonUp(0))
            {
                HandleDragEnd();
            }
        }

        void HandleDragEnd()
        {
            var allSlots = FindObjectsOfType<Slot>(includeInactive: false);

            var closestSlot = allSlots
                .Where(slot => RectTransformUtility.RectangleContainsScreenPoint(slot.RectTransform, Input.mousePosition))
                .OrderBy(slot => Vector2.Distance(slot.RectTransform.position, Input.mousePosition))
                .FirstOrDefault();

            if (closestSlot != null)
            {
                bool droppedToSameSlot = closestSlot == DragState.OriginSlot;

                // 드롭 대상 슬롯이 속한 View로부터 드롭 처리
                var targetContainer = closestSlot.GetComponentInParent<IItemContainer>();

                targetContainer?.HandleDrop(DragState.OriginSlot, closestSlot, DragState.DraggingItem);
                return;
                
                // if (!targetContainer?.HandleDrop(DragState.OriginSlot, closestSlot, DragState.DraggingItem) ?? true)
                // {
                //     DragState.OriginSlot?.RestoreVisual();
                // }


                if (droppedToSameSlot)
                {
                    DragState.OriginSlot?.RestoreVisual();
                }
            }
            else
            {
                DragState.OriginSlot?.RestoreVisual();
            }

            var view = DragState.OriginSlot?.GetComponentInParent<StorageView>();
            view?.HideGhostIcon();

            DragState.Clear();
        }
    }
}