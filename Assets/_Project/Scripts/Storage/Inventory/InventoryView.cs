using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Timelesss
{
    public class InventoryView : StorageView, IItemContainer
    {
        [SerializeField] string panelName = "인벤토리";
        [SerializeField] GameObject slotPrefab; // 슬롯 프리팹
        [SerializeField] Transform slotsParent; // 슬롯 부모 컨테이너
        //[SerializeField] GameObject ghostIconPrefab; // 고스트 아이콘 프리팹
        [SerializeField] Button closeButton; // 닫기 버튼
        [SerializeField] TextMeshProUGUI inventoryHeader; // 제목 텍스트
        
        InventoryController controller; //todo
        public override void InitializeView(int capacity = 0)
        {
            // 슬롯 배열 초기화
            Slots = new Slot[capacity];

            // 제목 설정
            inventoryHeader.text = panelName;

            // 슬롯 영역 UI 생성
            for (int i = 0; i < capacity; i++)
            {
                // 슬롯 프리팹 생성 및 부모 컨테이너에 추가
                var slotObject = Instantiate(slotPrefab, slotsParent);
                var slotComponent = slotObject.GetComponent<Slot>();

                Slots[i] = slotComponent;
                slotComponent.Initialize(i); // 초기화 (예: 인덱스 설정)
            }

            // // 고스트 아이콘 생성
            // if (ghostIconPrefab != null)
            // {
            //     var ghostIconObject = Instantiate(ghostIconPrefab, transform);
            //     ghostIcon = ghostIconObject.GetComponent<RectTransform>();
            //     ghostIcon.gameObject.SetActive(false); // 기본적으로 비활성화
            // }

            // 닫기 버튼 이벤트 연결
            closeButton.onClick.AddListener(ClosePopup);
            
        }
        
        protected override Item GetItemFromSlot(Slot slot)
        {
            return controller?.Model?.Get(slot.Index); // 인벤토리 기준
        }
        
        
        public void Bind(InventoryController controller)
        {
            this.controller = controller;
        }
        
   
        public bool HandleDrop(Slot fromSlot, Slot toSlot, Item item)
        {
            if (item == null || controller?.Model == null) return true;

            int fromIndex = fromSlot.Index;
            int toIndex = toSlot.Index;

            if (fromIndex == toIndex) return false;

            var model = controller.Model;
            var targetItem = model.Get(toIndex);

            if (targetItem != null &&
                targetItem.details.Id == item.details.Id &&
                targetItem.quantity + item.quantity <= item.details.MaxStack)
            {
                model.Combine(fromIndex, toIndex);
            }
            else
            {
                model.Swap(fromIndex, toIndex);
            }
            
            return true;
        }
    }
}