using System.Collections;
using TMPro;
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
            base.Interact();
            StartCoroutine(PickupItem());
        }
        IEnumerator PickupItem()
        {
            if(Clip != null) yield return new WaitForSeconds(Clip.length);
            
            itemEventChannel.Invoke(itemData);

            spawner.ReturnItem(this);
            
            InteractionManager.Instance.EndInteraction();
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
