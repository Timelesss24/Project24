using UnityEngine;

namespace Timelesss
{
    public class DropItem : InteractableBase
    {
        [SerializeField] private ItemData itemData;
        public ItemData ItemData {  get { return itemData; } }

        public override string InteractionName { get; } = "획득하기";

        private ItemSpawner spawner;

        [SerializeField] EventChannel<ItemData> itemEventChannel;

        public override void Interact()
        {
            Debug.Log($"{itemData} 획득");

            // 아이템 줍기 애니메이션

            // UI 프롬프트 띄워주기 (XX 를 획득했습니다.)

            itemEventChannel.Invoke(itemData);


            spawner.ReturnItem(this);
        }

        public void Initialize(ItemData data)
        {
            itemData = data;

            if( spawner == null )
            {
                spawner = FindObjectOfType<ItemSpawner>();

                if( spawner == null )
                {
                    Debug.LogError("현재 Scene에서 ItemSpawner를 찾을 수 없습니다.");
                }
            }
        }
    }
}
