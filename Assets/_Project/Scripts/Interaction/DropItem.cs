using UnityEngine;

namespace Timelesss
{
    public class DropItem : InteractableBase
    {
        [SerializeField] private GameObject item; // ������ SO ���� �� GameObject���� ���� ����

        public override string InteractionName { get; } = "ȹ���ϱ�";

        public override void Interact()
        {
            Debug.Log($"{item.name} ȹ��");

            // ������ �ݱ� �ִϸ��̼�

            // UI ������Ʈ ����ֱ� (XX �� ȹ���߽��ϴ�.)

            // inventory.AddItem(item);


            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }
}
