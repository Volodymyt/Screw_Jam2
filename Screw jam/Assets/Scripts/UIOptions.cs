using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIOptions : MonoBehaviour
{
    [SerializeField] int _boards;
    [SerializeField] private GameObject _winPanle, _losePanel;
    [SerializeField] private Sprite _soundButtonOffSprite, _soundButtonOnSprite;
    [SerializeField] private Image _soundsButtonIamge;
    [SerializeField] private GameObject _thisLevel;
    [SerializeField] private Text _levelName;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] float _time;

    private void Start()
    {
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

    public void LoadLevel(GameObject level)
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        Instantiate(level);
        Destroy(_thisLevel);
        Time.timeScale = 1;
    }

    public void Restart(GameObject level)
    {
        Destroy(_thisLevel);
        Instantiate(level);
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