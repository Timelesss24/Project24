using Managers;
using UnityEngine;

namespace Timelesss
{
    public class Portal : InteractableBase
    {
        public override string InteractionName { get; } = "�����ϱ�";

        [SerializeField] private string targetSceneName;

        public override void Interact()
        {
            // ���� Ȯ�� �˾� ����ֱ�

            // ConfirmPopup.SetUp("�������� �����Ͻðڽ��ϱ�?, () => SceneLoader.Instance.LoadScene(targetSceneName)");

            Debug.Log($"{targetSceneName} ���� ����");
            SceneLoader.Instance.LoadScene(targetSceneName); // �׽�Ʈ��
        }
    }
}
