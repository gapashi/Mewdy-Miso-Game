using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

[System.Serializable]
public class DialogueLine
{
    public string type;
    public string text;
    public List<string> audio;
    public bool stopHere;
    public string spriteTrigger;
}

[System.Serializable]
public class DialogueScene
{
    public string sceneName;
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueContainer
{
    public List<DialogueScene> scenes;
}

[System.Serializable]
public class DialogueData
{
    public List<DialogueLine> lines;
}

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public TextMeshProUGUI continueHint;
    public TextMeshProUGUI characterName;
    public AudioSource audioSource;

    public GameObject misoObject;
    public Sprite misoIdleSprite;
    public Sprite misoMeowSprite;
    public Sprite misoAngrySprite;
    public Sprite misoEatingSprite;
    public Sprite misoUpSprite;

    public GameObject closedBackground;
    public GameObject openButton;
    public GameObject openBackground;
    public GameObject brokenClosedBG;
    public GameObject brokenOpenBG;
    public GameObject chicken;
    public GameObject beef;
    public GameObject fish;
    public GameObject brokenChicken;
    public GameObject brokenBeef;
    public GameObject brokenFish;

    public string nameWritten = "John";
    public float typingSpeed = 0.03f;
    public bool ingredientsReady = false;

    private DialogueData dialogueData;
    private int currentLine = 0;
    private bool isTyping = false;
    private bool readyForNext = false;
    private bool isPaused = false;

    private const string ReloadedFlag = "SceneReloadedFromInteraction";

    void Start()
    {
        GameSettingsManager.LoadSettings();
        audioSource.volume = GameSettingsManager.Settings.sfxVolume;

        dialogueText.fontSize = GameSettingsManager.Settings.fontSize;
        continueHint.fontSize = GameSettingsManager.Settings.fontSize;
        characterName.fontSize = GameSettingsManager.Settings.fontSize;

        dialoguePanel.SetActive(true);
        continueHint.gameObject.SetActive(false);
        openButton.SetActive(false);

        bool isReloaded = PlayerPrefs.GetInt(ReloadedFlag, 0) == 1;

        if (isReloaded)
        {
            closedBackground.SetActive(false);
            openBackground.SetActive(false);
            brokenClosedBG.SetActive(true);
            brokenOpenBG.SetActive(false);
        }
        else
        {
            closedBackground.SetActive(true);
            openBackground.SetActive(false);
            brokenClosedBG.SetActive(false);
            brokenOpenBG.SetActive(false);
        }

        PlayerPrefs.DeleteKey(ReloadedFlag);

        LoadDialogueFromJSON();
        if (dialogueData != null && dialogueData.lines.Count > 0)
        {
            StartCoroutine(DisplayLine(dialogueData.lines[currentLine]));
        }
        else
        {
            Debug.LogError("Nenhuma linha de diálogo carregada!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueData.lines[currentLine].text;
                isTyping = false;
                readyForNext = true;
                continueHint.gameObject.SetActive(true);
            }
            else if (readyForNext && !isPaused)
            {
                currentLine++;
                if (currentLine < dialogueData.lines.Count)
                {
                    continueHint.gameObject.SetActive(false);
                    StartCoroutine(DisplayLine(dialogueData.lines[currentLine]));
                }
                else
                {
                    dialoguePanel.SetActive(false);
                }
            }
        }
    }

    public IEnumerator DisplayLine(DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";

        characterName.text = (line.type == "dialogue") ? nameWritten : "";

        if (line.audio != null && line.audio.Count > 0)
        {
            StartCoroutine(PlayAudioSequence(line.audio));
        }

        if (!string.IsNullOrEmpty(line.spriteTrigger) && misoObject != null)
        {
            SpriteRenderer renderer = misoObject.GetComponent<SpriteRenderer>();
            switch (line.spriteTrigger)
            {
                case "miso_idle":
                    renderer.sprite = misoIdleSprite;
                    break;
                case "miso_meow":
                    renderer.sprite = misoMeowSprite;
                    break;
                case "miso_angry":
                    renderer.sprite = misoAngrySprite;
                    break;
                case "miso_up":
                    renderer.sprite = misoUpSprite;
                    break;
                default:
                    Debug.LogWarning("SpriteTrigger não reconhecido: " + line.spriteTrigger);
                    break;
            }
        }

        foreach (char letter in line.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        readyForNext = true;
        continueHint.gameObject.SetActive(true);

        if (line.stopHere)
        {
            isPaused = true;

            if (brokenClosedBG.activeSelf)
            {
                brokenClosedBG.transform.Find("BrokenButton")?.gameObject.SetActive(true);
            }
            else
            {
                openButton.SetActive(true);
            }
        }
    }

    IEnumerator PlayAudioSequence(List<string> audioFiles)
    {
        foreach (string audioName in audioFiles)
        {
            if (string.IsNullOrEmpty(audioName)) continue;

            string cleanName = Path.GetFileNameWithoutExtension(audioName);
            AudioClip clip = Resources.Load<AudioClip>("Sounds/Voice/" + cleanName);
            if (clip != null)
            {
                audioSource.volume = GameSettingsManager.Settings.sfxVolume;
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(clip.length);
            }
            else
            {
                Debug.LogWarning("Áudio não encontrado: " + audioName);
            }
        }
    }

    void LoadDialogueFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogue.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            DialogueContainer container = JsonUtility.FromJson<DialogueContainer>(json);

            foreach (DialogueScene scene in container.scenes)
            {
                if (scene.sceneName == "gameScene")
                {
                    dialogueData = new DialogueData { lines = scene.lines };
                    break;
                }
            }

            if (dialogueData != null)
            {
                foreach (var line in dialogueData.lines)
                {
                    if (line.audio == null)
                        line.audio = new List<string>();
                }

                Debug.Log($"Cena 'gameScene' carregada com {dialogueData.lines.Count} linhas.");
            }
            else
            {
                Debug.LogError("Cena 'gameScene' não encontrada no JSON!");
                dialogueData = new DialogueData { lines = new List<DialogueLine>() };
            }
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado em: " + filePath);
            dialogueData = new DialogueData { lines = new List<DialogueLine>() };
        }
    }

    public void ContinueDialogue()
    {
        if (isPaused)
        {
            isPaused = false;
            continueHint.gameObject.SetActive(false);
            openButton.SetActive(false);
            currentLine++;
            if (currentLine < dialogueData.lines.Count)
            {
                StartCoroutine(DisplayLine(dialogueData.lines[currentLine]));
            }
            else
            {
                dialoguePanel.SetActive(false);
            }
        }
    }

    public void OnOpenButtonClicked()
    {
        closedBackground.SetActive(false);
        openBackground.SetActive(true);
        openButton.SetActive(false);

        ingredientsReady = true;

        if (fish != null) fish.SetActive(true);
        if (beef != null) beef.SetActive(true);
        if (chicken != null) chicken.SetActive(true);
    }

    public void OnOpenBrokenButtonClicked()
    {
        brokenClosedBG.SetActive(false);
        brokenOpenBG.SetActive(true);

        ingredientsReady = true;

        if (brokenFish != null) brokenFish.SetActive(true);
        if (brokenBeef != null) brokenBeef.SetActive(true);
        if (brokenChicken != null) brokenChicken.SetActive(true);
    }


    public bool IsTyping() => isTyping;
    public bool IsReadyForNext() => readyForNext;
    public void ResetContinue() => readyForNext = false;

    public IEnumerator PlaySegment(string segmentKey)
    {
        isPaused = true;
        dialoguePanel.SetActive(true);
        continueHint.gameObject.SetActive(false);

        List<DialogueLine> linesToPlay = GetSegmentLines(segmentKey);

        foreach (var line in linesToPlay)
        {
            yield return StartCoroutine(DisplayLine(line));

            while (isTyping) yield return null;
            while (!readyForNext) yield return null;

            ResetContinue();
        }

        isPaused = false;
        dialoguePanel.SetActive(false);
    }

    private List<DialogueLine> GetSegmentLines(string key)
    {
        var result = new List<DialogueLine>();
        bool match = false;

        foreach (var line in dialogueData.lines)
        {
            if (key == "beef" && line.text.Contains("Oh, this beef has real gravy"))
                match = true;
            if (key == "fish" && line.text.Contains("You can never go wrong with fish"))
                match = true;
            if (key == "chicken" && line.text.Contains("Okay, how about Chicken?"))
                match = true;

            if (match)
                result.Add(line);

            if (match && result.Count >= 8) break; // pode ajustar para mais ou menos
        }

        return result;
    }
}
