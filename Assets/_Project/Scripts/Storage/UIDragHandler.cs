using System;
using System.Linq;
using UnityEngine;
using UnityUtils;

namespace Timelesss
{
    public class UIDragHandler : Singleton<UIDragHandler>
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

            var originSlot = DragState.OriginSlot;
            var originContainer = DragState.OriginContainer;
            var item = DragState.DraggingItem;

            if (closestSlot)
            {
                bool droppedToSameSlot = closestSlot == originSlot;

                var targetContainer = closestSlot.GetComponentInParent<IItemContainer>();

                if (targetContainer?.HandleDrop(originSlot, closestSlot, item, item => {
                        // originSlot.Remove();
                        // originContainer.HandleDrop(closestSlot, originSlot, item);
                    }) ?? false)
                {
                    // 다른 컨테이너로 옮겨졌다면 원래 모델에서 제거
                    if (targetContainer != originContainer)
                    {
                        switch (originContainer)
                        {
                            case InventoryView inventoryView:
                                inventoryView.Controller?.Model?.Remove(item);
                                break;

                            case EquipmentView equipmentView:
                                equipmentView.Controller?.Model?.Remove(item);
                                equipmentView.Controller?.VisualHandler?.Unequip(item.Details.EquipmentType);
                                break;
                            case ConsumableStorage:
                                break;
                        }
                    }
                }
                else
                {
                    originSlot?.RestoreVisual();
                }

                if (droppedToSameSlot)
                {
                    originSlot?.RestoreVisual();
                }
            }
            else
            {
                originSlot?.RestoreVisual();
            }

            var view = originSlot?.GetComponentInParent<StorageView>();
            view?.HideGhostIcon();

            DragState.Clear();
        }
    }
}