namespace Timelesss
{
    public interface IInteractable
    {
        // ��ȣ�ۿ� ���� �Լ�
        public void Interact();

        // ��ȣ�ۿ� �̸� (UI ǥ�ÿ� ex: ��ȭ�ϱ�, ȹ���ϱ�, �����ϱ�)
        public string InteractionName { get; }
    }
}
