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
    
    List<NPCDoctorBehavior> Doctors = new List<NPCDoctorBehavior>();
    
    private GameObject PlayerGameObject;
    
    private PlayerInteraction playerInteraction;

    private bool isGameOver = false;

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
        GameObject[] DoctorArray = GameObject.FindGameObjectsWithTag("Doctor");
        foreach (GameObject doctor in DoctorArray)
        {
            Doctors.Add(doctor.GetComponent<NPCDoctorBehavior>());
        }
        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerInteraction = PlayerGameObject.GetComponent<PlayerInteraction>();
        isGameOver = false;
    }

    public void OnClickQuit()
    {
        isGameOver = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else

        Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInteraction)
        {
            //게임 끝났을 때 음악을 재생하지 않습니다.
            if (isGameOver)
                return;
            //게임 진행 중
            //잡혔을 때 음악을 틀어줍니다.
            if (playerInteraction.GetQte() <= 0)
            {
                musicManager.PlayRestrainedBGM();
                return;
            }

            // 어느 한 Doctor라도 뒤쫓고 있다면 쫓아옵니다.
            if (!IsAllDoctorsPatrol())
            {
                musicManager.PlayDetectedBGM();
                return;
            }

            // 모든 Doctor가 뒤쫓고 있지 않을 때의 음악을 틀어줍니다.
            musicManager.PlayPatrolBGM();
        }
    }
    
    private bool IsAllDoctorsPatrol()
    {
        foreach (NPCDoctorBehavior doctor in Doctors)
        {
            if (doctor.GetCurrentState() is NPCDoctorStateGrabPlayer || 
                doctor.GetCurrentState() is NPCDoctorStateFollowingPlayer)
            {
                return false;
            }
        }
        return true;
    }
}
