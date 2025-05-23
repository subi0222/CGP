using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicManagerScript : MonoBehaviour
{
    public static MusicManagerScript Instance;

    private AudioSource audioSource;
    public AudioClip PatrolBGM;
    public AudioClip DetectedBGM;
    public AudioClip RestrainedBGM;


    [Header("MusicManager")] public GameObject MusicManagerObject;

    private MusicManagerScript musicManager;

    List<NPCDoctorBehavior> Doctors = new List<NPCDoctorBehavior>();

    private GameObject PlayerGameObject;

    private PlayerInteraction playerInteraction;

    private bool isGameOver = false;

    public enum MusicState
    {
        STATE_GAME_START,
        STATE_NPC_PATROL,
        STATE_PLAYER_DETECTED,
        STATE_PLAYER_RESTRAINED,
        STATE_GAME_PAUSE,
        STATE_GAME_END
    }


    private struct EventActionEntry
    {
        public MusicState curState;
        public ActionDelegate canChangeState;
        public ActionDelegate action;
        public MusicState nextState;

        public delegate void ActionDelegate();

        public EventActionEntry(MusicState state, ActionDelegate canChange, ActionDelegate action, MusicState next)
        {
            curState = state;
            canChangeState = canChange;
            this.action = action;
            nextState = next;
        }
    }

    private EventActionEntry[] table = new EventActionEntry[]
    {
    };


void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        // 싱글톤 처리 (선택)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameStart()
    {
        GameObject[] DoctorArray = GameObject.FindGameObjectsWithTag("Doctor");
        foreach (GameObject doctor in DoctorArray)
        {
            Doctors.Add(doctor.GetComponent<NPCDoctorBehavior>());
        }
        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerInteraction = PlayerGameObject.GetComponent<PlayerInteraction>();
        isGameOver = false;
    }

    public void GameEnd()
    {
        isGameOver = true;
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

    private bool IsPlayerRestrained()
    {
        return playerInteraction.GetQte() <= 0;
    }
    
    

    public void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip) return; // 같은 음악이면 무시
        audioSource.clip = clip;
        audioSource.Play();
    }

    // 상황별 함수 예시
    public void PlayPatrolBGM() => PlayBGM(PatrolBGM);
    public void PlayDetectedBGM() => PlayBGM(DetectedBGM);
    public void PlayRestrainedBGM() => PlayBGM(RestrainedBGM);
}
