using UnityUtils;

namespace Timelesss
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public PlayerInfo PlayerIfo { get; private set; }
        public Inventory Inventory { get; private set; }
        public Equipment Equipment { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Inventory = GetComponent<Inventory>();
            Equipment = GetComponent<Equipment>();
            PlayerIfo = GetComponent<PlayerInfo>();
        }
    }
}