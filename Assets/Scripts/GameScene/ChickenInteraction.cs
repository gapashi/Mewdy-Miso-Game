using UnityEngine;

public class ChickenInteraction : MonoBehaviour
{
    public Transform bowl;
    public Transform bowlTarget;
    public DialogueManager dialogueManager;

    private bool isInteracting = false;

    void OnMouseDown()
    {
        if (isInteracting || dialogueManager == null || dialogueManager.IsTyping()) return;

        if (!dialogueManager.ingredientsReady) return;

        if (Vector3.Distance(bowl.position, bowlTarget.position) < 0.1f)
        {
            isInteracting = true;
            StartCoroutine(PlayDialogue());
        }
    }

    private System.Collections.IEnumerator PlayDialogue()
    {
        yield return dialogueManager.StartCoroutine(dialogueManager.PlaySegment("chicken"));
        isInteracting = false;
    }
}
