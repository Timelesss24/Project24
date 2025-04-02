using System;
using System.Collections.Generic;
using System.Linq;
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
            for (int i = 0; i < capacity; i++)
            {
                var item = Model.Get(i);
                SubscribeToItem(item);
            }

            RefreshView();
        }
        
        void SubscribeToItem(Item item)
        {
            if (item == null) return;

            item.OnChanged += RefreshView;
        }

        public void Bind(InventoryData data) => Model.Bind(data);
        

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
                    view.Slots[i].Set(item.Id, item.Details.Icon, item.Quantity);
                }
            }
        }

        #region Builder

        public class Builder
        {
            // readonly InventoryView view;
            IEnumerable<ItemDetails> itemDetails;
            Item[] items;
            int capacity = 20;
            
            public Builder WithStartingItems(IEnumerable<ItemDetails> itemDetails)
            {
                this.itemDetails = itemDetails;
                return this;
            }

            public Builder WithLoadItems(Item[] items)
            {
                Debug.Log("WithLoadItems 호출");
                
                this.items = items;
                return this;
            }
            
            public Builder WithCapacity(int capacity)
            {
                this.capacity = capacity;
                return this;
            }

            public InventoryController Build()
            {
                InventoryModel model;

                if (items != null)
                    model = new InventoryModel(items, capacity);
                else if (itemDetails != null)
                    model = new InventoryModel(itemDetails, capacity);
                else
                    model = new InventoryModel(Array.Empty<ItemDetails>(), capacity);

                return new InventoryController(model, capacity);
            }
        }

        #endregion Builder
    }
}