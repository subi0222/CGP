using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

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

    private bool bGameOver = false;

    private bool bGamePaused = false;

    private bool bGameStart = false;
    
    private bool bGameEnd = false;

    private MusicState curState = MusicState.STATE_GAME_START;

    public enum MusicState
    {
        STATE_GAME_START,
        STATE_NPC_PATROL,
        STATE_PLAYER_DETECTED,
        STATE_PLAYER_RESTRAINED,
        STATE_GAME_PAUSE,
        STATE_GAME_END
    }


    public struct EventActionEntry
    {
        public MusicState curState;
        public CheckDelegate canChangeState;
        public ActionDelegate action;
        public MusicState nextState;

        public delegate void ActionDelegate();

        public delegate bool CheckDelegate();

        public EventActionEntry(MusicState state, CheckDelegate canChange, ActionDelegate action, MusicState next)
        {
            curState = state;
            canChangeState = canChange;
            this.action = action;
            nextState = next;
        }
    }

    private EventActionEntry[] table;


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

        table = new EventActionEntry[]
        {
            new EventActionEntry(MusicState.STATE_GAME_START, IsGameStart, PlayPatrolBGM, MusicState.STATE_NPC_PATROL),
            new EventActionEntry(MusicState.STATE_NPC_PATROL, IsGamePaused, StopBGM, MusicState.STATE_GAME_PAUSE),
            new EventActionEntry(MusicState.STATE_NPC_PATROL, IsAllDoctorsPatrol, PlayPatrolBGM, MusicState.STATE_NPC_PATROL),
            new EventActionEntry(MusicState.STATE_NPC_PATROL, IsPlayerDetected, PlayDetectedBGM, MusicState.STATE_PLAYER_DETECTED),
            new EventActionEntry(MusicState.STATE_PLAYER_DETECTED, IsGamePaused, StopBGM, MusicState.STATE_GAME_PAUSE),
            new EventActionEntry(MusicState.STATE_PLAYER_DETECTED, IsPlayerRestrained, PlayRestrainedBGM, MusicState.STATE_PLAYER_RESTRAINED),
            new EventActionEntry(MusicState.STATE_PLAYER_DETECTED, IsPlayerDetected, PlayDetectedBGM, MusicState.STATE_PLAYER_DETECTED),
            new EventActionEntry(MusicState.STATE_PLAYER_DETECTED, IsAllDoctorsPatrol, PlayPatrolBGM, MusicState.STATE_NPC_PATROL),
            new EventActionEntry(MusicState.STATE_PLAYER_RESTRAINED, IsGameMainMenu, StopBGM, MusicState.STATE_GAME_START),
            new EventActionEntry(MusicState.STATE_PLAYER_RESTRAINED, IsGameRestart, PlayPatrolBGM, MusicState.STATE_NPC_PATROL),
            new EventActionEntry(MusicState.STATE_PLAYER_RESTRAINED, IsPlayerRestrained, PlayRestrainedBGM, MusicState.STATE_PLAYER_RESTRAINED),
            new EventActionEntry(MusicState.STATE_GAME_PAUSE, IsGameMainMenu, StopBGM, MusicState.STATE_GAME_START),
            new EventActionEntry(MusicState.STATE_GAME_PAUSE, IsGameRestart, PlayPatrolBGM, MusicState.STATE_NPC_PATROL),
            new EventActionEntry(MusicState.STATE_GAME_PAUSE, IsGameMainMenu, StopBGM, MusicState.STATE_GAME_START)
        };
    }

    public void Update()
    {
        for (int i = 0; i < table.Length; i++)
        {
            if (curState == table[i].curState && table[i].canChangeState())
            {
                table[i].action();
                curState = table[i].nextState;
            }
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
        bGameOver = false;
        bGameStart = true;
        bGamePaused = false;
    }

    public void GameEnd()
    {
        bGameOver = true;
        bGameStart = false;
        bGamePaused = false;
        bGameEnd = true;
    }

    public void GameOver()
    {
        bGameOver = true;
        bGameStart = false;
        bGamePaused = false;
        bGameEnd = false;
    }

    public void GamePause()
    {
        bGamePaused = true;
        bGameStart = true;
        bGameOver = false;
        bGameEnd = false;
    }

    public void GameResume()
    {
        bGamePaused = false;
        bGameStart = true;
        bGameOver = false;
        bGameEnd = false;
    }

    public void GameRestart()
    {
        bGameOver = false;
        bGameStart = true;
        bGamePaused = false;
        bGameEnd = false;
    }

    public void GameMainMenu()
    {
        bGameStart = false;
        bGameOver = false;
        bGamePaused = false;
        bGameEnd = false;
    }

    private bool IsGameOver()
    {
        return bGameOver;
    }

    private bool IsGameMainMenu()
    {
        return !bGameStart && !bGamePaused && !bGameEnd && !bGameOver;
    }

    private bool IsGamePaused()
    {
        return bGamePaused;
    }

    private bool IsGameRestart()
    {
        return bGameStart && !bGamePaused && !bGameEnd && !bGameOver;
    }

    private bool IsGameStart()
    {
        return bGameStart && !bGamePaused && !bGameEnd && !bGameOver;
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

    private bool IsPlayerDetected()
    {
        return !IsAllDoctorsPatrol();
    }

    private bool IsPlayerRestrained()
    {
        return playerInteraction.IsDead();
    }

    private bool IsGameResume()
    {
        return !bGamePaused && !bGameEnd && !bGameOver;
    }



    private void PlayBGM(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        if (audioSource.isPlaying && audioSource.clip != clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // 상황별 함수 예시
    public void PlayPatrolBGM() => PlayBGM(PatrolBGM);
    public void PlayDetectedBGM() => PlayBGM(DetectedBGM);
    public void PlayRestrainedBGM() => PlayBGM(RestrainedBGM);

    private void StopBGM()
    {
        audioSource.Stop();
    }
}
