using UnityEngine;
using UnityEngine.SceneManagement;

public class BeefInteraction : MonoBehaviour
{
    public Transform bowlTransform;
    public Transform bowlTarget;
    public DialogueManager dialogueManager;

    private bool activated = false;

    void OnMouseDown()
    {
        if (activated) return;

        if (dialogueManager == null)
            dialogueManager = Object.FindFirstObjectByType<DialogueManager>();

        if (!dialogueManager.ingredientsReady || dialogueManager.IsTyping()) return;

        if (Vector3.Distance(bowlTransform.position, bowlTarget.position) < 0.1f)
        {
            activated = true;
            StartCoroutine(PlayBeefSequence());
        }
    }

    private System.Collections.IEnumerator PlayBeefSequence()
    {
        DialogueManager manager = dialogueManager;
        SceneFader fader = Object.FindFirstObjectByType<SceneFader>();

        var beefLines = new System.Collections.Generic.List<DialogueLine>
        {
            new DialogueLine {
                type = "dialogue",
                text = "Oh, this beef has real gravy in it. Doesn't that sound nice, boy?",
                audio = new System.Collections.Generic.List<string> { "John_0120_tk01.mp3" }
            },
            new DialogueLine {
                type = "narration",
                text = "Miso stares. John proceeds to give Miso a bowl of wet, beef food. This causes a negative reaction from the cat.",
                spriteTrigger = "miso_angry"
            },
            new DialogueLine {
                type = "dialogue",
                text = "You don't like that? Uh..",
                audio = new System.Collections.Generic.List<string> { "John_0130_tk01.mp3" }
            },
            new DialogueLine {
                type = "narration",
                text = "Miso does not look happy. He's up to something."
            },
            new DialogueLine {
                type = "dialogue",
                text = "Wait. Miso, no! Not the..",
                audio = new System.Collections.Generic.List<string> { "John_0140_tk01.mp3" }
            },
            new DialogueLine {
                type = "narration",
                text = "Miso proceeds to drop a glass from the counter, and it lands on the floor.",
                audio = new System.Collections.Generic.List<string> { "sfx_glass_breaking.mp3" }
            },
            new DialogueLine {
                type = "dialogue",
                text = "..Glass.",
                audio = new System.Collections.Generic.List<string> { "John_0150_tk01.mp3" }
            },
            new DialogueLine {
                type = "dialogue",
                text = "I should’ve kept that piece of paper.",
                audio = new System.Collections.Generic.List<string> { "John_0160_tk01.mp3" }
            }
        };

        foreach (var line in beefLines)
        {
            yield return manager.StartCoroutine(manager.DisplayLine(line));
            while (manager.IsTyping()) yield return null;
            while (!manager.IsReadyForNext()) yield return null;
            manager.ResetContinue();
        }

        yield return new WaitForSeconds(0.5f);
        fader.FadeAndLoad("LoadingScene", 2f);
    }
}
