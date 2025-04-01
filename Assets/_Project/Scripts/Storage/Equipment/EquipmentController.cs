using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EquipmentController
    {
        EquipmentView view;
        public EquipmentModel Model { get; private set; }
        public EquipmentVisualHandler VisualHandler { get; private set; }

        EquipmentController(EquipmentModel model, EquipmentVisualHandler visualHandler)
        {
            Debug.Assert(model != null, "Model is null");
            Debug.Assert(visualHandler != null, "visualHandler is null");
            Model = model;
            VisualHandler = visualHandler;
        }

        public void InitializeView(EquipmentView view)
        {
            this.view = view;
            view.InitializeView();

            // view.OnDrop += HandleDrop;
            Model.OnModelChanged += HandleModelChanged;

            RefreshView();
        }

        public void Bind(EquipmentData data) => Model.Bind(data);

        // void HandleDrop(Slot originalSlot, Slot targetSlot)
        // {
        //     // 형변환 시도
        //     if (originalSlot is not EquipmentSlot originalEquipSlot || targetSlot is not EquipmentSlot targetEquipSlot) return;
        //     if (originalEquipSlot.EquipmentType != targetEquipSlot.EquipmentType)
        //         return; // 다른 부위엔 장착 불가
        //
        //     var item = Model.Get(originalEquipSlot.EquipmentType);
        //     if (item == null) return;
        //
        //     Model.Add(item); // 해당 부위에 장비 착용
        // }

        void HandleModelChanged(Dictionary<EquipmentType, Item> items)
        {
            RefreshView();

            foreach (var item in items.Values)
            {
                if (item == null) continue;
                VisualHandler?.Equip((EquipmentItem)item); // 3D 모델 장착
            }
        }

        void RefreshView()
        {
            for (int i = 0; i < view.Slots.Length; i++)
            {
                var type = (EquipmentType)i;
                var item = Model.Get(type);


                if (item == null || item.Id.Equals(SerializableGuid.Empty))
                {
                    view.Slots[i].Set(SerializableGuid.Empty, null);
                }
                else
                {
                    view.Slots[i].Set(item.Id, item.Details.Icon);
                }
            }
        }

        #region Builder

        public class Builder
        {
            IEnumerable<EquipmentDetails> itemDetails;
            EquipmentVisualHandler visualHandler;
            public Builder WithStartingItems(IEnumerable<EquipmentDetails> itemDetails)
            {
                this.itemDetails = itemDetails;
                return this;
            }

            public Builder WithVisualHandler(EquipmentVisualHandler handler)
            {
                this.visualHandler = handler;
                return this;
            }

            public EquipmentController Build()
            {
                var model = itemDetails != null
                    ? new EquipmentModel(itemDetails)
                    : new EquipmentModel(Array.Empty<EquipmentDetails>());


                foreach (var item in model.Items)
                    visualHandler.Equip((EquipmentItem)item.Value);


                return new EquipmentController(model, visualHandler);
            }
        }

        #endregion Builder
    }
}