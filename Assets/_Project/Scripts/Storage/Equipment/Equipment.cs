using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

namespace Timelesss
{
    public class Equipment : MonoBehaviour, IBind<EquipmentData>
    {
        [SerializeField] List<ItemDetails> startingItems = new(); // 초기 장비 아이템
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // 고유 ID

        public EquipmentController Controller { get; private set; }

        void Awake()
        {
            var visualHandler = GetComponent<EquipmentVisualHandler>();
            
            Controller = new EquipmentController.Builder()
                .WithStartingItems(startingItems)
                .WithVisualHandler(visualHandler)
                .Build();
        }

        public void Bind(EquipmentData data)
        {
            Debug.Log($"Binding equipment {Id} to data {data.Id}");
            Controller.Bind(data);
            data.Id = Id;
        }
    }
}