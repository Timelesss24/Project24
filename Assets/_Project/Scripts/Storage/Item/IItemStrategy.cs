namespace Timelesss
{
    public interface IItemStrategy
    {
        void Apply(PlayerInfo player);    // 사용 또는 장착
        void Remove(PlayerInfo player);   // 해제 (장착형만)
    }
    
    public class HealPotionStrategy : IItemStrategy
    {
        readonly float amount;
        public HealPotionStrategy(float amount)
        {
            this.amount = amount;
        }

        public void Apply(PlayerInfo player)
        {
            player.RestoreHealth(amount);
        }

        public void Remove(PlayerInfo player) { } // 사용 아이템은 제거 없음
    }
    
    public class EquipStrategy : IItemStrategy
    {
        readonly EquipItemData equipItemData;

        public EquipStrategy(EquipItemData equipItemData)
        {
            this.equipItemData = equipItemData;
        }

        public void Apply(PlayerInfo player)
        {
            player.ApplyEquipStatus(equipItemData);
            //player.GetComponent<PlayerEquip>().Equthis);
        }

        public void Remove(PlayerInfo player)
        {
            player.RemoveEquipStatus(equipItemData);
        }
    }
}