using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveHandler : MonoBehaviour
{
    [SerializeField] private int _levelNumber, _levelIndex;

    private void Awake()
    {
        SaveSystem.Init();

    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SaveLastLevelIndex();
        }

        Load();
    }

    public void Save()
    {
        CombinedSaveClass saveClass = LoadData();

        saveClass.LevelNumber = _levelNumber + 1;

        string json = JsonUtility.ToJson(saveClass);
        SaveSystem.Save(json);

        Debug.Log($"Saved: {json}");
    }

    public void SaveLastLevelIndex()
    {
        CombinedSaveClass saveClass = LoadData();

        saveClass.LastLevelIndex = SceneManager.GetActiveScene().buildIndex;

        string json = JsonUtility.ToJson(saveClass);
        SaveSystem.Save(json);

        Debug.Log($"Saved: {json}");
    }

    public void Load()
    {
        CombinedSaveClass saveClass = LoadData();

        if (saveClass != null)
        {
            _levelNumber = saveClass.LevelNumber;
            _levelIndex = saveClass.LastLevelIndex;
        }
    }

    private CombinedSaveClass LoadData()
    {
        string saveString = SaveSystem.Load();

        if (!string.IsNullOrEmpty(saveString))
        {
            return JsonUtility.FromJson<CombinedSaveClass>(saveString);
        }

        return new CombinedSaveClass();
    }

    public int ReturnSavedLevel()
    {
        return _levelNumber;
    }

    public int ReturnLastLevelIndex()
    {
        return _levelIndex;
    }

    [System.Serializable]
    private class CombinedSaveClass
    {
        public int LevelNumber;
        public int LastLevelIndex;
    }
}
