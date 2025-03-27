namespace Timelesss
{
    public interface IInteractable
    {
        // 상호작용 수행 함수
        public void Interact();

        // 상호작용 이름 (UI 표시용 ex: 대화하기, 획득하기, 입장하기)
        public string InteractionName { get; }
    }
}
