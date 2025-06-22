using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSettings
{
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float fontSize = 36f;
}

public static class GameSettingsManager
{
    private static string filePath = Path.Combine(Application.persistentDataPath, "settings.json");
    public static GameSettings Settings { get; private set; } = new GameSettings();

    static GameSettingsManager()
    {
        LoadSettings();
    }

    public static void SaveSettings()
    {
        string json = JsonUtility.ToJson(Settings, true);
        File.WriteAllText(filePath, json);
    }

    public static void LoadSettings()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Settings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            Settings = new GameSettings(); 
            SaveSettings();
        }
    }
}
