using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Timelesss
{
    public class WheelOnlyScrollRect : ScrollRect
    {
        public override void OnBeginDrag(PointerEventData eventData) { }
        public override void OnDrag(PointerEventData eventData) { }
        public override void OnEndDrag(PointerEventData eventData) { }
    }
}
