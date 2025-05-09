using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public GameObject qteUI;
    public Slider qteSlider;
    
    private void Start()
    {
        qteUI.gameObject.SetActive(false);
    }

    public void SetQteSlider(bool enable)
    {
        qteUI.gameObject.SetActive(enable);
    }

    public void SetQteValue(float value)
    {
        qteSlider.value = value;
    }
}
