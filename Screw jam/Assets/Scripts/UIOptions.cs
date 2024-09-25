using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private int _maxScenes;
    [SerializeField] private GameObject _winPanle, _losePanel;
    [SerializeField] private Sprite _soundButtonOffSprite, _soundButtonOnSprite;
    [SerializeField] private Image _soundsButtonIamge;
    [SerializeField] private GameObject _thisLevel;
    [SerializeField] private Text _levelName;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private bool _loadLevelRandom = false;
    [SerializeField] private int _boards;
    [SerializeField] private float _time;

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("MaxLevel1"));
        if (!PlayerPrefs.HasKey("MaxLevel1"))
        {
            PlayerPrefs.SetInt("MaxLevel1", 0);
        }

        if (PlayerPrefs.GetInt("MaxLevel1") == 1)
        {
            _loadLevelRandom = true;
        }
        else
        {
            _loadLevelRandom = false;
        }

        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            PlayerPrefs.SetInt("MaxLevel1", 1);
        }

        int lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

        Time.timeScale = 1;

        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 1);
        }

        if (_loadLevelRandom)
        {
            if (!PlayerPrefs.HasKey("Scene"))
            {
                PlayerPrefs.SetInt("Scene", SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                int savedSceneIndex = PlayerPrefs.GetInt("Scene");

                if (savedSceneIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    SceneManager.LoadScene(savedSceneIndex);
                }
            }
        }
        else
        {
            if (!PlayerPrefs.HasKey("Scene"))
            {
                PlayerPrefs.SetInt("Scene", SceneManager.GetActiveScene().buildIndex);
            }

            if (PlayerPrefs.GetInt("Scene") != SceneManager.GetActiveScene().buildIndex)
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt("Scene"));
            }
        }


        _levelName.text = "Level " + PlayerPrefs.GetInt("Level");

        Board[] boardComponents = FindObjectsOfType<Board>();

        _boards = boardComponents.Length;
    }

    private void Update()
    {
        _time -= Time.deltaTime;

        if (_time <= 0)
        {
            OpenPanel(_losePanel);
        }
        else
        {
            _timer.text = _time.ToString("F1");
        }
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        Time.timeScale = 1;
    }

    public void LoadLevel()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);

        if (_loadLevelRandom)
        {
            int levelNumber = 0;
            do
            {
                levelNumber = Random.Range(0, _maxScenes);
            } while (levelNumber == SceneManager.GetActiveScene().buildIndex);

            PlayerPrefs.SetInt("Scene", levelNumber);
            SceneManager.LoadScene(levelNumber);
        }
        else
        {
            PlayerPrefs.SetInt("Scene", PlayerPrefs.GetInt("Scene") + 1);
            SceneManager.LoadScene(PlayerPrefs.GetInt("Scene"));
        }

        Time.timeScale = 1;
    }

    public void Restart(int level)
    {
        SceneManager.LoadScene(level);
        Time.timeScale = 1;
    }

    public void ChangeAudio(AudioSource audioSource)
    {
        if (audioSource.volume == 1)
        {
            audioSource.volume = 0;
            _soundsButtonIamge.sprite = _soundButtonOffSprite;

        }
        else if (audioSource.volume == 0)
        {
            audioSource.volume = 1;
            _soundsButtonIamge.sprite = _soundButtonOnSprite;
        }
    }

    public void RecoundBourds()
    {
        _boards--;

        if (_boards <= 0)
        {
            StartCoroutine(LoadWinPanel());
        }
    }

    private IEnumerator LoadWinPanel()
    {
        yield return new WaitForSeconds(0.9f);

        _winPanle.SetActive(true);
        Time.timeScale = 0;
    }
}