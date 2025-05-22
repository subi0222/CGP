using UnityEngine;
using UnityEngine.UI;

//UI 관련 스크립트입니다.
public class UIManager : MonoBehaviour
{
    public GameObject qteUI;
    public Slider qteSlider;

    public GameObject deathUI;
    public GameObject GameStartUI;
    public GameObject gamePauseUI;
    
    public GameObject musicManager;
    
    public void Start()
    {
        qteUI.gameObject.SetActive(false);
        deathUI.gameObject.SetActive(false);
        GameStartUI.gameObject.SetActive(false);
        gamePauseUI.gameObject.SetActive(false);
    }

    public void SetGamePauseUI(bool enable)
    {
        var audioSource = musicManager.GetComponent<AudioSource>();
        audioSource.mute = enable;
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        gamePauseUI.gameObject.SetActive(enable);
        Time.timeScale = enable ? 0 : 1;
    }

    public void SetGameStartUI(bool enable)
    {
        GameStartUI.gameObject.SetActive(enable);
    }

    public void SetDeathUI(bool enable)
    {
        deathUI.gameObject.SetActive(enable);
    }

    public void SetQteUI(bool enable)
    {
        qteUI.gameObject.SetActive(enable);
    }

    public void SetQteValue(float value)
    {
        qteSlider.value = value;
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else

        Application.Quit();
#endif
    }
}
