using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Cursor Sprites")]
    public Texture2D cursorDefault;
    public Texture2D cursorInteract;

    [Header("Hotspot")]
    public Vector2 hotspot = Vector2.zero;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(cursorDefault, hotspot, CursorMode.Auto);
    }

    public void SetInteractCursor()
    {
        Cursor.SetCursor(cursorInteract, hotspot, CursorMode.Auto);
    }
}
