using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private GameObject[] _levels;
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
        Time.timeScale = 1;

        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 1);
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

    private void LoadLevelRandom()
    {
        if (_loadLevelRandom)
        {
            Instantiate(_levels[Random.Range(0, _levels.Length)]);
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
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        /* PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
         Time.timeScale = 1;
         Instantiate(_levels[1]);
         Destroy(_thisLevel);

         Debug.Log("ok");*/
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