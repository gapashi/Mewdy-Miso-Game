using UnityEngine;

public class CursorWorldInteract : MonoBehaviour
{
    void OnMouseEnter()
    {
        CursorManager.Instance?.SetInteractCursor();
    }

    void OnMouseExit()
    {
        CursorManager.Instance?.SetDefaultCursor();
    }
}
