using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ClockDialogueLine
{
    public string type;
    public string text;
    public List<string> audio;
}

[System.Serializable]
public class ClockSceneDialogue
{
    public string sceneName;
    public List<ClockDialogueLine> lines;
}

[System.Serializable]
public class ClockDialogueData
{
    public List<ClockSceneDialogue> scenes;
}

public class ClockDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI clockTextUI;
    public TextMeshProUGUI clockCharNameUI;
    public GameObject clockDialoguePanel;
    public TextMeshProUGUI clockContinueHint;
    public AudioSource clockAudioSource;

    public string clockSceneKey = "clockScene";
    public float clockTypingSpeed = 0.03f;

    private List<ClockDialogueLine> clockLines;
    private int clockLineIndex = 0;

    private bool clockIsTyping = false;
    private bool clockReadyForNext = false;

    void Start()
    {
        GameSettingsManager.LoadSettings();

        clockAudioSource.volume = GameSettingsManager.Settings.sfxVolume;

        clockTextUI.fontSize = GameSettingsManager.Settings.fontSize;
        clockContinueHint.fontSize = GameSettingsManager.Settings.fontSize;
        clockCharNameUI.fontSize = GameSettingsManager.Settings.fontSize;

        clockDialoguePanel.SetActive(false);
    }
    public void StartClockDialogue()
    {
        clockDialoguePanel.SetActive(true);
        clockContinueHint.gameObject.SetActive(false);

        LoadClockDialogue();

        if (clockLines != null && clockLines.Count > 0)
        {
            StartCoroutine(DisplayClockLine(clockLines[clockLineIndex]));
        }
        else
        {
            Debug.LogWarning("Nenhuma fala encontrada para a cena do relógio.");
            clockDialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (clockIsTyping)
            {
                StopAllCoroutines();
                clockTextUI.text = clockLines[clockLineIndex].text;
                clockIsTyping = false;
                clockReadyForNext = true;
                clockContinueHint.gameObject.SetActive(true);
            }
            else if (clockReadyForNext)
            {
                clockLineIndex++;
                if (clockLineIndex < clockLines.Count)
                {
                    clockContinueHint.gameObject.SetActive(false);
                    StartCoroutine(DisplayClockLine(clockLines[clockLineIndex]));
                }
                else
                {
                    clockDialoguePanel.SetActive(false);
                    SceneManager.LoadScene("IntroGame");
                }
            }
        }
    }

    void LoadClockDialogue()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogue.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ClockDialogueData allClockData = JsonUtility.FromJson<ClockDialogueData>(json);

            ClockSceneDialogue clockScene = allClockData.scenes.Find(s => s.sceneName == clockSceneKey);
            if (clockScene != null)
            {
                clockLines = clockScene.lines;
                foreach (var line in clockLines)
                {
                    if (line.audio == null)
                        line.audio = new List<string>();
                }
            }
            else
            {
                Debug.LogError("Cena do relógio não encontrada no JSON.");
                clockLines = new List<ClockDialogueLine>();
            }
        }
        else
        {
            Debug.LogError("Arquivo dialogue.json não encontrado.");
            clockLines = new List<ClockDialogueLine>();
        }
    }

    IEnumerator DisplayClockLine(ClockDialogueLine line)
    {
        clockIsTyping = true;
        clockTextUI.text = "";

        if (line.type == "dialogue")
            clockCharNameUI.text = "John";
        else
            clockCharNameUI.text = "";

        if (line.text == "An alarm clock goes off in the background.")
        {
            Object.FindFirstObjectByType<TimeProgressionController>().StopBlinkingAndFixSprite();
        }

        if (line.audio != null && line.audio.Count > 0)
            StartCoroutine(PlayClockAudio(line.audio));

        foreach (char letter in line.text.ToCharArray())
        {
            clockTextUI.text += letter;
            yield return new WaitForSeconds(clockTypingSpeed);
        }

        clockIsTyping = false;
        clockReadyForNext = true;
        clockContinueHint.gameObject.SetActive(true);
    }

    IEnumerator PlayClockAudio(List<string> audioClips)
    {
        foreach (string audioName in audioClips)
        {
            AudioClip clip = Resources.Load<AudioClip>("Sounds/Voice/" + Path.GetFileNameWithoutExtension(audioName));
            if (clip != null)
            {
                clockAudioSource.clip = clip;
                clockAudioSource.Play();
                yield return new WaitForSeconds(clip.length);
            }
            else
            {
                Debug.LogWarning("Áudio não encontrado: " + audioName);
            }
        }
    }
}
