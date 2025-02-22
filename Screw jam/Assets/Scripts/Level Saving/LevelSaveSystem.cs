using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string _saveFolder =
    Application.platform == RuntimePlatform.Android
        ? Application.persistentDataPath + "/Saves"
        : Application.dataPath + "/Saves";

    public static void Init()
    {
        if (!Directory.Exists(_saveFolder))
        {
            Directory.CreateDirectory(_saveFolder);
        }
    }

    public static void Save(string saveString)
    {
        File.WriteAllText(_saveFolder + "/save.txt", saveString);
    }

    public static string Load()
    {
        if (File.Exists(_saveFolder + "/save.txt"))
        {
            string saveString = File.ReadAllText(_saveFolder + "/save.txt");

            return saveString;
        }
        else
        {
            return null;
        }
    }
}
