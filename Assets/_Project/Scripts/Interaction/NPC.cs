using UnityEngine;

namespace Timelesss
{
    public class NPC : InteractableBase
    {
        public override string InteractionName { get; } = "��ȭ�ϱ�";

        public override void Interact()
        {
            // �÷��̾� ���� �Ұ��� �ϵ��� ����

            // NPC �ٶ󺸵��� ȸ��

            // UI ������Ʈ �����ֱ�

            // ��ȭâ UI ����

            Debug.Log("NPC�� ��ȭ");
        }
    }
}

