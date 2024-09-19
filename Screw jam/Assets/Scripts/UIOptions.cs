using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIOptions : MonoBehaviour
{
    [SerializeField] int _boards;
    [SerializeField] private GameObject _winPanle;
    [SerializeField] private Sprite _soundButtonOffSprite, _soundButtonOnSprite;
    [SerializeField] private Image _soundsButtonIamge;

    private void Start()
    {
        Board[] boardComponents = FindObjectsOfType<Board>();

        _boards = boardComponents.Length;
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

    public void OpenScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
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