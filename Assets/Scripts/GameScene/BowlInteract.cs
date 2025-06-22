using UnityEngine;

public class BowlInteract : MonoBehaviour
{
    [Header("Posição Alternativa")]
    public Transform targetPosition;
    private Vector3 originalPosition;
    private bool moved = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (!moved)
        {
            if (targetPosition != null)
            {
                transform.position = targetPosition.position;
                moved = true;
            }
        }
        else
        {
            transform.position = originalPosition;
            moved = false;
        }
    }
}
