using System;
using Systems.Persistence;
using UnityEngine;

namespace Timelesss
{
    [Serializable]
    public class EquipmentData : ISaveable
    { 
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public Item[] Items;
    }
}