using UnityEngine;
using UnityEngine.UI;

//UI 관련 스크립트입니다.
public class UIManager : MonoBehaviour
{
    public GameObject qteUI;
    public Slider qteSlider;

    public GameObject deathUI;
    
    private void Start()
    {
        qteUI.gameObject.SetActive(false);
        deathUI.gameObject.SetActive(false);
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
}
