using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [Header("UIManger")]
    public GameObject UIMangerObject;
    
    private UIManager uiManager;
    
    [Header("MapGenerator")]
    public GameObject MapGeneratorObject;
    
    private MapGenerator mapGenerator;

    public void Awake()
    {
        uiManager = UIMangerObject.GetComponent<UIManager>();
        mapGenerator = MapGeneratorObject.GetComponent<MapGenerator>();
    }
    public void Start()
    {
        uiManager.SetGameStartUI(true);
        uiManager.SetDeathUI(false);
        uiManager.SetQteUI(false);
    }

    public void OnClickStart()
    {
        uiManager.SetGameStartUI(false);
        uiManager.SetDeathUI(false);
        uiManager.SetQteUI(false);
        mapGenerator.StartMapGeneration();
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else

        Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
