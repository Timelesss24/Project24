using Systems.Persistence;

namespace Timelesss
{
    public class EquipmentData : ISaveable
    { 
        public SerializableGuid Id { get; set; }
        public Item[] Items;
    }
}