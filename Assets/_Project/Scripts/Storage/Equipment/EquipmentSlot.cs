using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class EquipmentSlot : Slot
    {
        [field:SerializeField]public EquipmentType EquipmentType { get; private set; }
        public IItemContainer Container { get; private set; }
        // public void Initialize(EquipmentType type)
        // {
        //     EquipmentType = type;
        //     base.Initialize((int)type);
        // }
        public void Initialize(EquipmentType type, IItemContainer container)
        {
            EquipmentType = type;
            this.Container = container;
            base.Initialize((int)type);
        }
    }
}