using UnityEngine;
using UnityEngine.SceneManagement;

public class FishInteraction : MonoBehaviour
{
    public Transform bowl;
    public Transform bowlTarget;
    public DialogueManager dialogueManager;

    private bool isInteracting = false;

    void OnMouseDown()
    {
        if (isInteracting) return;

        if (dialogueManager == null)
            dialogueManager = Object.FindFirstObjectByType<DialogueManager>();

        if (dialogueManager == null || dialogueManager.IsTyping()) return;

        if (!dialogueManager.ingredientsReady) return;

        if (Vector3.Distance(bowl.position, bowlTarget.position) < 0.1f)
        {
            isInteracting = true;
            StartCoroutine(PlayDialogue());
        }
    }

    private System.Collections.IEnumerator PlayDialogue()
    {
        yield return dialogueManager.StartCoroutine(dialogueManager.PlaySegment("fish"));
        isInteracting = false;

        yield return new WaitForSeconds(0.5f);

        SceneFader fader = Object.FindFirstObjectByType<SceneFader>();
        if (fader != null)
            fader.FadeAndLoad("LoadingScene", 2f);
        else
            SceneManager.LoadScene("LoadingScene");
    }
}
