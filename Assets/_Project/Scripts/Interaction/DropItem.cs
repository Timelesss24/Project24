using UnityEngine;

namespace Timelesss
{
    public class DropItem : InteractableBase
    {
        [SerializeField] private GameObject item; // 아이템 SO 생성 후 GameObject에서 변경 예정

        public override string InteractionName { get; } = "획득하기";

        public override void Interact()
        {
            Debug.Log($"{item.name} 획득");

            // 아이템 줍기 애니메이션

            // UI 프롬프트 띄워주기 (XX 를 획득했습니다.)

            // inventory.AddItem(item);


            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }
}
