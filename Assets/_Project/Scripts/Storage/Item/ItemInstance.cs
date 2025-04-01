using UnityEngine;

namespace Timelesss
{
    public class ItemInstance : MonoBehaviour
    {
    public ItemData Data { get; private set; }
    IItemStrategy strategy;

    public void Use(PlayerInfo player)
    {
        strategy ??= Data.CreateStrategy();
        strategy.Apply(player);
    }

    public void UnEquip(PlayerInfo player)
    {
        strategy?.Remove(player);
    }
    }
}