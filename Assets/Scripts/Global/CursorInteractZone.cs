using UnityEngine;
using UnityEngine.EventSystems;

public class CursorInteractZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance?.SetInteractCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance?.SetDefaultCursor();
    }
}
