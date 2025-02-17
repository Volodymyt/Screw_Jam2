using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private int _maxScenes;
    [SerializeField] private GameObject _winPanle, _losePanel, _additionalWinPanel;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Image[] _imagesWitchNeedToChangeTheColor;
    [SerializeField] private Image _startPanel;
    [SerializeField] private SaveHandler _saveHandler;

    [SerializeField] private Sprite _soundButtonOffSprite, _soundButtonOnSprite, _vibrateButtonOffSprite, _vibrateButtonOnSprite;
    [SerializeField] private Image _soundsButtonIamge, _vibrateButtonImage;
    [SerializeField] private GameObject _thisLevel;
    [SerializeField] private TMP_Text _timer, _startLevelText, _levelName, _winLevelText, _nextLevelText;
    [SerializeField] private bool _loadLevelRandom = false;
    [SerializeField] private int _boards;
    [SerializeField] private float _time, _fadeDuration;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            _startLevelText.text = "Level " + PlayerPrefs.GetInt("LevelIndex").ToString();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (PlayerPrefs.GetInt("LevelIndex") > 0)
            {
                _startLevelText.text = "Level " + (PlayerPrefs.GetInt("LevelIndex") - 1).ToString();
            }
        }
    }

    private void Start()
    {
        StartCoroutine(LoadLevelsText());
        StartCoroutine(CloseStartPanel());
        StartCoroutine(CheckScenes());

        if (!PlayerPrefs.HasKey("Audio"))
        {
            PlayerPrefs.SetInt("Audio", 1);
        }

        if (PlayerPrefs.GetInt("Audio") == 1)
        {
            _audioSource.volume = 1;
            _soundsButtonIamge.sprite = _soundButtonOnSprite;
        }
        else
        {
            _audioSource.volume = 0;
            _soundsButtonIamge.sprite = _soundButtonOffSprite;
        }

        if (!PlayerPrefs.HasKey("Vibrate"))
        {
            PlayerPrefs.SetInt("Vibrate", 1);
        }

        if (PlayerPrefs.GetInt("Vibrate") == 1)
        {
            _vibrateButtonImage.sprite = _vibrateButtonOnSprite;
        }
        else
        {
            _vibrateButtonImage.sprite = _vibrateButtonOffSprite;
        }

        Time.timeScale = 1;

        Board[] boardComponents = FindObjectsOfType<Board>();

        _boards = boardComponents.Length;
    }

    private void Update()
    {
        if (_time >= 0)
        {
            int minutes = Mathf.FloorToInt(_time / 60);
            int seconds = Mathf.FloorToInt(_time % 60);

            _timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            _time -= Time.deltaTime;
        }
        else
        {
            _losePanel.SetActive(true);
            _timer.text = "00:00";
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        if (_saveHandler.ReturnSavedLevel() < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(_saveHandler.ReturnSavedLevel() + 1);
        }
        else
        {
            int randomLevel = Random.Range(1, SceneManager.sceneCountInBuildSettings);

            Debug.Log(randomLevel);
            SceneManager.LoadScene(randomLevel);
        }

        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void ChangeAudio()
    {
        if (PlayerPrefs.GetInt("Audio") == 1)
        {
            _audioSource.volume = 0;
            _soundsButtonIamge.sprite = _soundButtonOffSprite;
            PlayerPrefs.SetInt("Audio", 0);

        }
        else if (PlayerPrefs.GetInt("Audio") == 0)
        {
            _audioSource.volume = 1;
            _soundsButtonIamge.sprite = _soundButtonOnSprite;
            PlayerPrefs.SetInt("Audio", 1);
        }
    }

    public void ChangeVibrate()
    {
        if (PlayerPrefs.GetInt("Vibrate") == 1)
        {
            _vibrateButtonImage.sprite = _vibrateButtonOffSprite;
            PlayerPrefs.SetInt("Vibrate", 0);
        }
        else
        {
            _vibrateButtonImage.sprite = _vibrateButtonOnSprite;
            PlayerPrefs.SetInt("Vibrate", 1);
        }
    }

    public void RecoundBourds(int count)
    {
        _boards -= count;

        if (_boards <= 0)
        {
            StartCoroutine(LoadWinPanel());
        }
    }

    private IEnumerator LoadLevelsText()
    {
        yield return new WaitForSeconds(0f);

        if (_saveHandler.ReturnSavedLevel() == 0)
        {
            _startLevelText.text = "Tutorial Level";
            _winLevelText.text = "Tutorial Level";
            _levelName.text = "Tutorial Level";
        }
        else
        {
            _startLevelText.text = "Level " + _saveHandler.ReturnSavedLevel().ToString();
            _winLevelText.text = "Level " + _saveHandler.ReturnSavedLevel().ToString();
            _levelName.text = "Level " + _saveHandler.ReturnSavedLevel();
        }
        _nextLevelText.text = "Level " + (_saveHandler.ReturnSavedLevel() + 1).ToString();

        PlayerPrefs.SetInt("LevelIndex", _saveHandler.ReturnSavedLevel() + 1);
    }

    private IEnumerator LoadWinPanel()
    {
        _winPanle.SetActive(true);
        _saveHandler.Save();

        yield return new WaitForSeconds(1f);
        _additionalWinPanel.SetActive(true);
        yield return new WaitForSeconds(0.8f);

        foreach (Image image in _imagesWitchNeedToChangeTheColor)
        {
            if (image != null)
                StartCoroutine(FadeAlpha(image, 1f, 0f, _fadeDuration));
        }

        if (_levelName != null)
            StartCoroutine(FadeAlpha(_levelName, 1f, 0f, _fadeDuration));

        yield return new WaitForSeconds(2.2f);

        LoadLevel();
    }

    private IEnumerator CheckScenes()
    {
        yield return new WaitForSeconds(0.05f);

        if (_saveHandler.ReturnLastLevelIndex() != SceneManager.GetActiveScene().buildIndex)
        {
            SceneManager.LoadScene(_saveHandler.ReturnLastLevelIndex());
        }
    }

    private IEnumerator CloseStartPanel()
    {
        yield return new WaitForSeconds(0.3f);

        StartCoroutine(HideStartPanle(_startPanel, 1f, 0f, 1));

        StartCoroutine(HideStartPanle(_startLevelText, 1f, 0f, 1));

        yield return new WaitForSeconds(1.1f);

        _startPanel.gameObject.SetActive(false);
    }

    private IEnumerator HideStartPanle(Graphic image, float startAlpha, float endAlpha, float duration)
    {
        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    private IEnumerator FadeAlpha(Graphic graphic, float startAlpha, float endAlpha, float duration)
    {
        Color color = graphic.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            graphic.color = color;
            yield return null;
        }

        color.a = endAlpha;
        graphic.color = color;
    }
}