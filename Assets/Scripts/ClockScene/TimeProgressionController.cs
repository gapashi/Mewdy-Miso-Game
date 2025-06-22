using UnityEngine;
using UnityEngine.UI;

public class TimeProgressionController : MonoBehaviour
{
    public GameObject[] spriteObjects;
    public float secondsPerSprite = 3f;
    public float blinkInterval = 0.5f;

    public ClockDialogueManager dialogueManager;

    private int currentIndex = 0;
    private float timer = 0f;
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private bool dialogueStarted = false;
    private bool blinkingStoppedManually = false;

    void Start()
    {
        for (int i = 0; i < spriteObjects.Length; i++)
        {
            spriteObjects[i].SetActive(false);
        }
        if (spriteObjects.Length > 0)
        {
            spriteObjects[0].SetActive(true);
        }
    }

    void Update()
    {
        if (!isBlinking)
        {
            timer += Time.deltaTime;

            if (timer >= secondsPerSprite)
            {
                timer = 0f;

                spriteObjects[currentIndex].SetActive(false);
                currentIndex++;

                if (currentIndex >= spriteObjects.Length)
                {
                    currentIndex = spriteObjects.Length - 1;
                    isBlinking = true;
                    spriteObjects[currentIndex].SetActive(true);
                    blinkTimer = 0f;

                    // 👇 Iniciar diálogo aqui
                    if (!dialogueStarted && dialogueManager != null)
                    {
                        dialogueManager.StartClockDialogue();
                        dialogueStarted = true;
                    }
                }
                else
                {
                    spriteObjects[currentIndex].SetActive(true);
                }
            }
        }
        else if (!blinkingStoppedManually) 
        {
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;
                bool isActive = spriteObjects[currentIndex].activeSelf;
                spriteObjects[currentIndex].SetActive(!isActive);
            }
        }
    }

    public void StopBlinkingAndFixSprite()
    {
        blinkingStoppedManually = true;
        isBlinking = false;

        if (currentIndex >= 0 && currentIndex < spriteObjects.Length)
        {
            spriteObjects[currentIndex].SetActive(true);
        }
    }
}
