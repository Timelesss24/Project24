using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EquipmentController
    {
        EquipmentView view;
        public EquipmentModel Model { get; private set; }

        EquipmentController(EquipmentModel model)
        {
            Debug.Assert(model != null, "Model is null");
            this.Model = model;
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
        }

        void RefreshView()
        {
            for (int i = 0; i < view.Slots.Length; i++)
            {
                var type = (EquipmentType)i;
                var item = Model.Get(type);

                
                if (item == null || item.Id.Equals(SerializableGuid.Empty))
                {
                    view.Slots[i].Set(SerializableGuid.Empty, ((EquipmentSlot)view.Slots[i]).Icon.sprite);
                }
                else
                {
                    view.Slots[i].Set(item.Id, item.details.Icon);
                }
            }
        }

        #region Builder

        public class Builder
        {
            IEnumerable<ItemDetails> itemDetails;

            public Builder WithStartingItems(IEnumerable<ItemDetails> itemDetails)
            {
                this.itemDetails = itemDetails;
                return this;
            }

            public EquipmentController Build()
            {
                var model = itemDetails != null
                    ? new EquipmentModel(itemDetails)
                    : new EquipmentModel(Array.Empty<ItemDetails>());

                return new EquipmentController(model);
            }
        }

        #endregion Builder
    }
}