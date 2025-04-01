using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
     public class InventoryController
    {
        InventoryView view;
        public InventoryModel Model { get; private set; }
        readonly int capacity;

        InventoryController(InventoryModel model, int capacity)
        {
            Debug.Assert(model != null, "Model is null");
            Debug.Assert(capacity > 0, "Capacity is less than 1");
            this.Model = model;
            this.capacity = capacity;
        }

        public void InitializeView(InventoryView view)
        {
            this.view = view;
            view.InitializeView(capacity);

            //view.OnDrop += HandleDrop;
            Model.OnModelChanged += HandleModelChanged;

            RefreshView();
        }

        public void Bind(InventoryData data) => Model.Bind(data);

        // void HandleDrop(Slot originalSlot, Slot closestSlot)
        // {
        //     // Moving to Same Slot or Empty Slot
        //     if (originalSlot.Index == closestSlot.Index || closestSlot.ItemId.Equals(SerializableGuid.Empty))
        //     {
        //         Model.Swap(originalSlot.Index, closestSlot.Index);
        //         return;
        //     }
        //
        //     // TODO world drops
        //     // TODO Cross Inventory drops
        //     // TODO HotBar drops
        //
        //     // Moving to Non-Empty Slot
        //     var sourceItemId = Model.Get(originalSlot.Index).details.Id;
        //     var targetItemId = Model.Get(closestSlot.Index).details.Id;
        //
        //     if (sourceItemId.Equals(targetItemId) 
        //         && Model.Get(closestSlot.Index).quantity + Model.Get(originalSlot.Index).quantity <= Model.Get(closestSlot.Index).details.MaxStack)
        //     {
        //         Model.Combine(originalSlot.Index, closestSlot.Index);
        //     }
        //     else
        //     {
        //         Model.Swap(originalSlot.Index, closestSlot.Index);
        //     }
        // }

        void HandleModelChanged(IList<Item> items) => RefreshView();

        void RefreshView()
        {
            
            
            for (int i = 0; i < capacity; i++)
            {
                var item = Model.Get(i);
                if (item == null || item.Id.Equals(SerializableGuid.Empty))
                {
                    view.Slots[i].Set(SerializableGuid.Empty, null);
                }
                else
                {
                    view.Slots[i].Set(item.Id, item.details.Icon, item.quantity);
                }
            }
        }

        #region Builder

        public class Builder
        {
            // readonly InventoryView view;
            IEnumerable<ItemDetails> itemDetails;
            int capacity = 20;
            
            public Builder WithStartingItems(IEnumerable<ItemDetails> itemDetails)
            {
                this.itemDetails = itemDetails;
                return this;
            }

            public Builder WithCapacity(int capacity)
            {
                this.capacity = capacity;
                return this;
            }

            public InventoryController Build()
            {
                var model = itemDetails != null
                    ? new InventoryModel(itemDetails, capacity)
                    : new InventoryModel(Array.Empty<ItemDetails>(), capacity);

                return new InventoryController(model, capacity);
            }
        }

        #endregion Builder
    }
}