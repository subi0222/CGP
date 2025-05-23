using System.Collections.Generic;
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
    
    [Header("MusicManager")]
    public GameObject MusicManagerObject;
    
    private MusicManagerScript musicManager;
    
    private PlayerInteraction playerInteraction;
    
    public void Awake()
    {
        uiManager = UIMangerObject.GetComponent<UIManager>();
        mapGenerator = MapGeneratorObject.GetComponent<MapGenerator>();
        musicManager = MusicManagerObject.GetComponent<MusicManagerScript>();
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
        musicManager.GameStart();
        playerInteraction = GameObject.Find("Player").GetComponent<PlayerInteraction>();
    }

    public void OnClickRestart()
    {
        //Debug.Log("Pressed");
        uiManager.SetGamePauseUI(false);
        musicManager.GameRestart();
        DestroyImmediate(mapGenerator.transform.GetChild(0).gameObject);
        DestroyImmediate(playerInteraction.gameObject);
        OnClickStart();
    }

    public void OnClickMainMenu()
    {
        uiManager.SetGamePauseUI(false);
        musicManager.GameQuit();
        Cursor.lockState = CursorLockMode.None;
        DestroyImmediate(mapGenerator.transform.GetChild(0).gameObject);
        DestroyImmediate(playerInteraction.gameObject);
        Start();
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
